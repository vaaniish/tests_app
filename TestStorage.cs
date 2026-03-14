using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class TestStorage
{
    private const string DbFileName = "tests_secure.db";
    private const string CollectionName = "tests";
    private const int MinimumDefaultTests = 5;
    private const int MinimumQuestionsPerDefaultTest = 50;
    private const int MinimumQuestionsInFinalMixedTest = 150;
    private const int TargetTextQuestionsPerBuiltIn = 8;
    private const int TargetMultiQuestionsPerBuiltIn = 12;
    private const string FinalTestId = "tm7-final-advanced";
    private const string FinalAutoPrefix = "final-auto::";
    private static readonly string[] FinalSourceTestIds =
    {
        "tm7-level-1",
        "tm7-level-2",
        "tm7-level-3",
        "tm7-level-4-5"
    };
    private static readonly string[] PreferredOrder =
    {
        "tm7-level-1",
        "tm7-level-2",
        "tm7-level-3",
        "tm7-level-4-5",
        "tm7-final-advanced"
    };
    private static readonly Lazy<Dictionary<string, List<string>>> DefaultAnswerMap =
        new Lazy<Dictionary<string, List<string>>>(BuildDefaultAnswerMap);

    // РџР°СЂРѕР»СЊ С€РёС„СЂРѕРІР°РЅРёСЏ РІСЃС‚СЂРѕРµРЅРЅРѕР№ Р‘Р” (AES РІРЅСѓС‚СЂРё LiteDB).
    private const string DbPassword = "TM7_SECURE_DB_2026_!x9Yp4@mQ2";

    public static List<Test> LoadOrCreateDefaultTests(string baseDirectory)
    {
        var tests = LoadTests(baseDirectory);
        if (tests != null && tests.Count > 0)
        {
            var changedByMigration = MigrateToEncryptedOnly(tests);
            var changedByQuestionMixUpgrade = UpgradeBuiltInQuestionMixIfNeeded(tests);
            if (EnsureRequiredTestsExist(tests))
            {
                SaveTests(baseDirectory, tests);
                return LoadTests(baseDirectory);
            }

            if (changedByMigration || changedByQuestionMixUpgrade)
            {
                SaveTests(baseDirectory, tests);
                return LoadTests(baseDirectory);
            }

            return tests;
        }

        var defaults = DefaultTestCatalog.Create();
        SaveTests(baseDirectory, defaults);
        return LoadTests(baseDirectory);
    }

    public static List<Test> LoadTests(string baseDirectory)
    {
        EnsureDatabaseInitialized(baseDirectory);

        var dbPath = GetDbPath(baseDirectory);
        if (!File.Exists(dbPath))
            return new List<Test>();

        using (var db = OpenDatabase(dbPath))
        {
            var col = db.GetCollection<Test>(CollectionName);
            var tests = col.FindAll().ToList();
            return NormalizeLoaded(tests);
        }
    }

    public static void SaveTests(string baseDirectory, List<Test> tests)
    {
        var dbPath = GetDbPath(baseDirectory);
        var normalized = NormalizeForSave(tests ?? new List<Test>());

        using (var db = OpenDatabase(dbPath))
        {
            var col = db.GetCollection<Test>(CollectionName);
            col.DeleteAll();
            col.InsertBulk(normalized);
            col.EnsureIndex(x => x.Id, true);
            db.Checkpoint();
        }
    }

    private static void EnsureDatabaseInitialized(string baseDirectory)
    {
        var dbPath = GetDbPath(baseDirectory);
        if (File.Exists(dbPath))
            return;

        // РќРµ РјРёРіСЂРёСЂСѓРµРј Р°РІС‚РѕРјР°С‚РёС‡РµСЃРєРё legacy tests.enc РЅР° СЃС‚Р°СЂС‚Рµ:
        // СЌС‚Рѕ РјРѕР¶РµС‚ СЃРёР»СЊРЅРѕ С‚РѕСЂРјРѕР·РёС‚СЊ Р·Р°РїСѓСЃРє Рё Р±Р»РѕРєРёСЂРѕРІР°С‚СЊ РѕС‚РѕР±СЂР°Р¶РµРЅРёРµ С„РѕСЂРјС‹.
        var defaults = DefaultTestCatalog.Create();
        SaveTests(baseDirectory, defaults);
        CleanupLegacyFiles(baseDirectory);
    }

    private static void CleanupLegacyFiles(string baseDirectory)
    {
        var candidates = new[]
        {
            Path.Combine(baseDirectory, "tests.enc"),
            Path.Combine(baseDirectory, "tests.json"),
            Path.Combine(baseDirectory, "tests_debug.json")
        };

        foreach (var file in candidates)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch
            {
                // РРіРЅРѕСЂРёСЂСѓРµРј: СЃС‚Р°СЂС‹Рµ С„Р°Р№Р»С‹ РЅРµ РєСЂРёС‚РёС‡РЅС‹ РґР»СЏ СЂР°Р±РѕС‚С‹ Р‘Р”.
            }
        }
    }

    private static LiteDatabase OpenDatabase(string dbPath)
    {
        var cs = new ConnectionString
        {
            Filename = dbPath,
            Password = DbPassword,
            Upgrade = true,
            Connection = ConnectionType.Shared
        };

        return new LiteDatabase(cs);
    }

    private static string GetDbPath(string baseDirectory)
    {
        return Path.Combine(baseDirectory, DbFileName);
    }

    private static bool IsDefaultPoolActual(List<Test> tests)
    {
        if (tests == null || tests.Count < MinimumDefaultTests)
            return false;

        foreach (var id in PreferredOrder)
        {
            var test = tests.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.Ordinal));
            if (test == null)
                return false;

            var count = (test.Questions ?? new List<Question>()).Count;

            if (id == "tm7-final-advanced")
            {
                if (count < MinimumQuestionsInFinalMixedTest)
                    return false;
                continue;
            }

            if (count < MinimumQuestionsPerDefaultTest)
                return false;
        }

        return true;
    }

    private static bool EnsureRequiredTestsExist(List<Test> tests)
    {
        if (tests == null)
            return false;

        var defaults = DefaultTestCatalog.Create()
            .ToDictionary(t => t.Id, StringComparer.Ordinal);

        var changed = false;
        foreach (var id in PreferredOrder)
        {
            if (tests.Any(t => string.Equals(t.Id, id, StringComparison.Ordinal)))
                continue;

            if (!defaults.TryGetValue(id, out var source))
                continue;

            tests.Add(CloneTest(source));
            changed = true;
        }

        return changed;
    }

    private static Test CloneTest(Test source)
    {
        return new Test
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            TimeMinutes = source.TimeMinutes,
            Questions = (source.Questions ?? new List<Question>())
                .Select(CloneQuestion)
                .ToList()
        };
    }

    private static Question CloneQuestion(Question source)
    {
        return new Question
        {
            Id = source.Id,
            Text = source.Text,
            Type = source.Type,
            Options = source.Options != null ? new List<string>(source.Options) : new List<string>(),
            Answer = source.Answer,
            AnswerEncrypted = source.AnswerEncrypted,
            FinalSourceKey = source.FinalSourceKey,
            AnswerHash = source.AnswerHash,
            AnswerSalt = source.AnswerSalt
        };
    }

    private static List<Test> NormalizeLoaded(List<Test> tests)
    {
        foreach (var test in tests)
        {
            test.Id = string.IsNullOrWhiteSpace(test.Id)
                ? "test-" + Guid.NewGuid().ToString("N").Substring(0, 8)
                : test.Id;
            test.Title = test.Title ?? string.Empty;
            test.Description = test.Description ?? string.Empty;
            test.TimeMinutes = test.TimeMinutes > 0 ? test.TimeMinutes : 20;
            test.Questions = test.Questions ?? new List<Question>();

            foreach (var q in test.Questions)
            {
                q.Id = string.IsNullOrWhiteSpace(q.Id) ? Guid.NewGuid().ToString() : q.Id;
                q.Text = q.Text ?? string.Empty;
                q.Options = q.Options ?? new List<string>();
                q.Answer = q.Answer ?? string.Empty;
                q.AnswerEncrypted = q.AnswerEncrypted ?? string.Empty;
                q.FinalSourceKey = q.FinalSourceKey ?? string.Empty;
                q.AnswerHash = q.AnswerHash ?? string.Empty;
                q.AnswerSalt = q.AnswerSalt ?? string.Empty;

                if (q.Type == QuestionType.Text)
                    q.Options = new List<string>();
            }

            test.Questions = EnsureUniqueQuestionList(test.Questions);
        }

        SynchronizeFinalMixedTest(tests);

        return tests
            .OrderBy(t => Array.IndexOf(PreferredOrder, t.Id) < 0 ? int.MaxValue : Array.IndexOf(PreferredOrder, t.Id))
            .ThenBy(t => t.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<Test> NormalizeForSave(List<Test> tests)
    {
        var normalized = NormalizeLoaded(tests);
        SynchronizeFinalMixedTest(normalized);

        foreach (var test in normalized)
        {
            foreach (var q in test.Questions)
            {
                if (!string.IsNullOrWhiteSpace(q.Answer))
                {
                    SecurityService.ProtectQuestionAnswer(q);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(q.AnswerEncrypted))
                {
                    q.AnswerHash = string.Empty;
                    q.AnswerSalt = string.Empty;
                }
            }
        }

        return normalized;
    }

    public static bool TryRecoverEncryptedAnswer(Question question)
    {
        if (question == null)
            return false;

        if (!string.IsNullOrWhiteSpace(question.AnswerEncrypted))
            return true;

        var before = question.AnswerEncrypted ?? string.Empty;
        TryBackfillEncryptedAnswerFromDefaults(question);

        if (!string.IsNullOrWhiteSpace(question.AnswerEncrypted))
        {
            question.AnswerHash = string.Empty;
            question.AnswerSalt = string.Empty;
        }

        return !string.Equals(before, question.AnswerEncrypted ?? string.Empty, StringComparison.Ordinal);
    }

    private static bool MigrateToEncryptedOnly(List<Test> tests)
    {
        if (tests == null || tests.Count == 0)
            return false;

        var changed = false;
        foreach (var test in tests)
        {
            foreach (var q in test.Questions ?? new List<Question>())
            {
                if (!string.IsNullOrWhiteSpace(q.Answer))
                {
                    SecurityService.ProtectQuestionAnswer(q);
                    changed = true;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(q.AnswerEncrypted) &&
                    !string.IsNullOrWhiteSpace(q.AnswerHash))
                {
                    if (TryRecoverEncryptedAnswer(q))
                    {
                        changed = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(q.AnswerEncrypted) &&
                    (!string.IsNullOrWhiteSpace(q.AnswerHash) || !string.IsNullOrWhiteSpace(q.AnswerSalt)))
                {
                    q.AnswerHash = string.Empty;
                    q.AnswerSalt = string.Empty;
                    changed = true;
                }
            }
        }

        return changed;
    }

    private static bool UpgradeBuiltInQuestionMixIfNeeded(List<Test> tests)
    {
        if (tests == null || tests.Count == 0)
            return false;

        var defaults = DefaultTestCatalog.Create()
            .Where(t => !string.Equals(t.Id, FinalTestId, StringComparison.Ordinal))
            .ToDictionary(t => t.Id, StringComparer.Ordinal);

        var changed = false;
        foreach (var id in FinalSourceTestIds)
        {
            if (!defaults.TryGetValue(id, out var defaultTest))
                continue;

            var current = tests.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.Ordinal));
            if (current == null)
                continue;

            var currentQuestions = current.Questions ?? new List<Question>();
            var currentText = currentQuestions.Count(q => q.Type == QuestionType.Text);
            var currentMulti = currentQuestions.Count(q => q.Type == QuestionType.Multiple);
            var hasLegacySymptomWording = currentQuestions.Any(q =>
                (q.Text ?? string.Empty).IndexOf("РЎРёРјРїС‚РѕРј:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                (q.Text ?? string.Empty).IndexOf("РёРЅР¶РµРЅРµСЂРЅРѕ РєРѕСЂСЂРµРєС‚РЅС‹Р№ С€Р°Рі", StringComparison.OrdinalIgnoreCase) >= 0);
            var hasLegacyLevelTitle =
                (id == "tm7-level-2" &&
                 (current.Title ?? string.Empty).IndexOf("SIAD-архив", StringComparison.OrdinalIgnoreCase) >= 0) ||
                (id == "tm7-level-4-5" &&
                 (current.Title ?? string.Empty).IndexOf("Документы и Web-доступ", StringComparison.OrdinalIgnoreCase) >= 0);
            var hasOutdatedReadableVersion =
                (current.Description ?? string.Empty)
                    .IndexOf(DefaultTestCatalog.ReadableCatalogVersion, StringComparison.OrdinalIgnoreCase) < 0;
            var hasCatalogTextMismatch =
                !string.Equals((current.Title ?? string.Empty).Trim(), (defaultTest.Title ?? string.Empty).Trim(), StringComparison.Ordinal) ||
                !string.Equals((current.Description ?? string.Empty).Trim(), (defaultTest.Description ?? string.Empty).Trim(), StringComparison.Ordinal);


            var needsUpgrade =
                currentQuestions.Count < MinimumQuestionsPerDefaultTest ||
                currentText < TargetTextQuestionsPerBuiltIn ||
                currentMulti < TargetMultiQuestionsPerBuiltIn ||
                hasLegacySymptomWording ||
                hasLegacyLevelTitle ||
                hasOutdatedReadableVersion ||
                hasCatalogTextMismatch;

            if (!needsUpgrade)
                continue;

            current.Title = defaultTest.Title;
            current.Description = defaultTest.Description;
            current.TimeMinutes = defaultTest.TimeMinutes;
            current.Questions = defaultTest.Questions
                .Select(CloneQuestion)
                .ToList();

            changed = true;
        }

        return changed;
    }

    private static void SynchronizeFinalMixedTest(List<Test> tests)
    {
        if (tests == null || tests.Count == 0)
            return;

        var sourceTests = FinalSourceTestIds
            .Select(id => tests.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.Ordinal)))
            .Where(t => t != null)
            .ToList();

        if (sourceTests.Count != FinalSourceTestIds.Length)
            return;

        var finalTest = tests.FirstOrDefault(t => string.Equals(t.Id, FinalTestId, StringComparison.Ordinal));
        if (finalTest == null)
        {
            finalTest = new Test
            {
                Id = FinalTestId,
                Title = "РС‚РѕРіРѕРІС‹Р№: СЃРјРµС€Р°РЅРЅС‹Р№ РїРѕ 4 РїСЂРµРґС‹РґСѓС‰РёРј С‚РµСЃС‚Р°Рј",
                Description = "РС‚РѕРіРѕРІС‹Р№ С‚РµСЃС‚ Р°РІС‚РѕРјР°С‚РёС‡РµСЃРєРё СЃРѕР±РёСЂР°РµС‚СЃСЏ РёР· СѓСЂРѕРІРЅРµР№ 1-4 Рё РјРѕР¶РµС‚ СЃРѕРґРµСЂР¶Р°С‚СЊ РґРѕРїРѕР»РЅРёС‚РµР»СЊРЅС‹Рµ РІРѕРїСЂРѕСЃС‹ РїСЂРµРїРѕРґР°РІР°С‚РµР»СЏ.",
                TimeMinutes = 20,
                Questions = new List<Question>()
            };
            tests.Add(finalTest);
        }

        finalTest.Questions = finalTest.Questions ?? new List<Question>();
        finalTest.Title = "Итоговый: смешанный по уровням 1-4";
        finalTest.Description = "Формируется смешиванием вопросов из уровней 1, 2, 3 и 4-5. Дополнительные вопросы преподавателя сохраняются.";

        finalTest.TimeMinutes = finalTest.TimeMinutes > 0 ? finalTest.TimeMinutes : 20;

        var sourceQuestions = sourceTests
            .SelectMany(t => (t.Questions ?? new List<Question>())
                .Select(q => new { TestId = t.Id, Question = q }))
            .GroupBy(x => BuildQuestionUniqueKey(x.Question), StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();

        var sourceFingerprints = new HashSet<string>(
            sourceQuestions.Select(x => BuildQuestionUniqueKey(x.Question)),
            StringComparer.Ordinal
        );

        var hasTaggedAutoQuestions = finalTest.Questions.Any(q => !string.IsNullOrWhiteSpace(q.FinalSourceKey));

        // РЎРѕС…СЂР°РЅСЏРµРј С‚РѕР»СЊРєРѕ РІСЂСѓС‡РЅСѓСЋ РґРѕР±Р°РІР»РµРЅРЅС‹Рµ РІРѕРїСЂРѕСЃС‹ РёС‚РѕРіРѕРІРѕРіРѕ С‚РµСЃС‚Р°.
        // Legacy-Р°РІС‚РѕРјР°С‚РёС‡РµСЃРєРёРµ РІРѕРїСЂРѕСЃС‹ (Р±РµР· FinalSourceKey) РѕС‚С„РёР»СЊС‚СЂРѕРІС‹РІР°СЋС‚СЃСЏ РїРѕ СЃРѕРІРїР°РґР°СЋС‰РµРјСѓ РѕС‚РїРµС‡Р°С‚РєСѓ.
        var manualQuery = finalTest.Questions
            .Where(q => string.IsNullOrWhiteSpace(q.FinalSourceKey));

        if (!hasTaggedAutoQuestions)
        {
            manualQuery = manualQuery
                .Where(q => !sourceFingerprints.Contains(BuildQuestionUniqueKey(q)));
        }

        var manualQuestions = manualQuery
            .Select(CloneQuestionForManualFinal)
            .ToList();

        var autoQuestions = sourceQuestions
            .Select(x => CloneQuestionForFinalAuto(x.TestId, x.Question))
            .ToList();

        finalTest.Questions = EnsureUniqueQuestionList(
            autoQuestions
                .Concat(manualQuestions)
                .ToList()
        );
    }

    private static Question CloneQuestionForFinalAuto(string sourceTestId, Question source)
    {
        return new Question
        {
            Id = BuildAutoQuestionId(sourceTestId, source.Id),
            Text = source.Text,
            Type = source.Type,
            Options = source.Options != null ? new List<string>(source.Options) : new List<string>(),
            Answer = source.Answer,
            AnswerEncrypted = source.AnswerEncrypted,
            AnswerHash = source.AnswerHash,
            AnswerSalt = source.AnswerSalt,
            FinalSourceKey = BuildSourceKey(sourceTestId, source.Id)
        };
    }

    private static Question CloneQuestionForManualFinal(Question source)
    {
        return new Question
        {
            Id = string.IsNullOrWhiteSpace(source.Id) ? Guid.NewGuid().ToString() : source.Id,
            Text = source.Text,
            Type = source.Type,
            Options = source.Options != null ? new List<string>(source.Options) : new List<string>(),
            Answer = source.Answer,
            AnswerEncrypted = source.AnswerEncrypted,
            AnswerHash = source.AnswerHash,
            AnswerSalt = source.AnswerSalt,
            FinalSourceKey = string.Empty
        };
    }

    private static string BuildSourceKey(string testId, string questionId)
    {
        return (testId ?? string.Empty) + "::" + (questionId ?? string.Empty);
    }

    private static string BuildAutoQuestionId(string testId, string questionId)
    {
        return FinalAutoPrefix + BuildSourceKey(testId, questionId);
    }

    private static string BuildQuestionFingerprint(Question q)
    {
        var typePart = ((int)q.Type).ToString();
        var textPart = NormalizeFingerprintPart(q.Text);
        var optionsPart = string.Join(";", (q.Options ?? new List<string>())
            .Select(NormalizeFingerprintPart)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x, StringComparer.Ordinal));

        return typePart + "|" + textPart + "|" + optionsPart;
    }

    private static string NormalizeFingerprintPart(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static string BuildQuestionUniqueKey(Question q)
    {
        return ((int)q.Type).ToString() + "|" + NormalizeFingerprintPart(q.Text);
    }

    private static List<Question> EnsureUniqueQuestionList(List<Question> questions)
    {
        var result = new List<Question>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var question in questions ?? new List<Question>())
        {
            var fingerprint = BuildQuestionUniqueKey(question);
            if (!seen.Add(fingerprint))
                continue;

            result.Add(question);
        }

        return result;
    }

    private static void TryBackfillEncryptedAnswerFromDefaults(Question q)
    {
        if (q == null)
            return;

        if (!string.IsNullOrWhiteSpace(q.AnswerEncrypted))
            return;

        if (string.IsNullOrWhiteSpace(q.AnswerHash) || string.IsNullOrWhiteSpace(q.AnswerSalt))
            return;

        var fingerprint = BuildQuestionFingerprint(q);
        if (!DefaultAnswerMap.Value.TryGetValue(fingerprint, out var candidates))
            return;

        // Р‘С‹СЃС‚СЂР°СЏ РјРёРіСЂР°С†РёСЏ legacy hash->enc РїРѕ РѕС‚РїРµС‡Р°С‚РєСѓ РІРѕРїСЂРѕСЃР°.
        // РџСЂРѕРІРµСЂРєСѓ hash Р·РґРµСЃСЊ РЅРµ РґРµР»Р°РµРј, С‡С‚РѕР±С‹ РЅРµ Р±Р»РѕРєРёСЂРѕРІР°С‚СЊ Р·Р°РїСѓСЃРє UI.
        var candidate = candidates.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(candidate))
        {
            q.AnswerEncrypted = SecurityService.EncryptAnswerForStorage(candidate);
        }
    }

    private static Dictionary<string, List<string>> BuildDefaultAnswerMap()
    {
        var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var defaults = DefaultTestCatalog.Create();

        foreach (var test in defaults)
        {
            foreach (var question in test.Questions ?? new List<Question>())
            {
                var key = BuildQuestionFingerprint(question);
                if (!map.TryGetValue(key, out var answers))
                {
                    answers = new List<string>();
                    map[key] = answers;
                }

                var answerValue = question.Answer ?? string.Empty;
                if (!answers.Any(x => string.Equals(x, answerValue, StringComparison.Ordinal)))
                {
                    answers.Add(answerValue);
                }
            }
        }

        return map;
    }

}
