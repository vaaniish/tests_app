using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class TestStorage
{
    private const string DbFileName = "tests_secure.db";
    private const string CollectionName = "tests";
    private const int MinimumQuestionsPerDefaultTest = 1;
    private const int MinimumQuestionsInFinalMixedTest = 20;
    private const string FinalTestId = "tm7-final-advanced";
    private const string FinalAutoPrefix = "final-auto::";
    private static readonly string[] LegacyBuiltInIds =
    {
        "tm7-level-1",
        "tm7-level-2",
        "tm7-level-3",
        "tm7-level-4-5"
    };
    private static readonly string[] LegacyBuiltInTitles =
    {
        "Уровень 1: Базовые понятия TRACE MODE 7",
        "Уровень 2: СПАД (SIAD)-архив и события",
        "Уровень 3: MODBUS, DPA и режимы управления",
        "Уровни 4-5: Документы, Web-доступ и ИБ АСУТП",
        "Итоговый: смешанный по уровням 1-4"
    };
    private static readonly Lazy<List<Test>> DefaultCatalogSnapshot =
        new Lazy<List<Test>>(DefaultTestCatalog.Create);
    private static readonly Lazy<string[]> PreferredOrder =
        new Lazy<string[]>(() => DefaultCatalogSnapshot.Value.Select(t => t.Id).ToArray());
    private static readonly Lazy<string[]> FinalSourceTestIds =
        new Lazy<string[]>(() => DefaultCatalogSnapshot.Value
            .Where(t => !string.Equals(t.Id, FinalTestId, StringComparison.Ordinal))
            .Select(t => t.Id)
            .ToArray());
    private static readonly Lazy<Dictionary<string, List<string>>> DefaultAnswerMap =
        new Lazy<Dictionary<string, List<string>>>(BuildDefaultAnswerMap);

    // Секрет подгружается из security.keys.json рядом с EXE.
    private static string DbPassword => RuntimeSecrets.DatabasePassword;

    public static List<Test> LoadOrCreateDefaultTests(string baseDirectory)
    {
        var tests = LoadTests(baseDirectory);
        if (tests != null && tests.Count > 0)
        {
            var changedByMigration = MigrateToEncryptedOnly(tests);
            var changedByLegacyCleanup = RemoveLegacyBuiltInTests(tests);
            var changedByQuestionMixUpgrade = UpgradeBuiltInQuestionMixIfNeeded(tests);
            if (EnsureRequiredTestsExist(tests))
            {
                SaveTests(baseDirectory, tests);
                return LoadTests(baseDirectory);
            }

            if (changedByMigration || changedByLegacyCleanup || changedByQuestionMixUpgrade)
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

        // Не мигрируем автоматически legacy tests.enc на старте:
        // это может сильно тормозить запуск и блокировать отображение формы.
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
                // Игнорируем: старые файлы не критичны для работы БД.
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
        var requiredIds = PreferredOrder.Value;
        if (tests == null || tests.Count < requiredIds.Length)
            return false;

        foreach (var id in requiredIds)
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
        foreach (var id in PreferredOrder.Value)
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

        var preferredOrder = PreferredOrder.Value;
        return tests
            .OrderBy(t => Array.IndexOf(preferredOrder, t.Id) < 0 ? int.MaxValue : Array.IndexOf(preferredOrder, t.Id))
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

        var defaults = DefaultCatalogSnapshot.Value
            .Where(t => !string.Equals(t.Id, FinalTestId, StringComparison.Ordinal))
            .ToDictionary(t => t.Id, StringComparer.Ordinal);

        var changed = false;
        foreach (var id in FinalSourceTestIds.Value)
        {
            if (!defaults.TryGetValue(id, out var defaultTest))
                continue;

            var current = tests.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.Ordinal));
            if (current == null)
                continue;

            var currentQuestions = current.Questions ?? new List<Question>();
            var defaultQuestions = defaultTest.Questions ?? new List<Question>();
            var defaultDescriptionHasVersionLabel =
                (defaultTest.Description ?? string.Empty)
                    .IndexOf("Версия банка:", StringComparison.OrdinalIgnoreCase) >= 0;
            var hasOutdatedReadableVersion =
                defaultDescriptionHasVersionLabel &&
                (current.Description ?? string.Empty)
                    .IndexOf(DefaultTestCatalog.ReadableCatalogVersion, StringComparison.OrdinalIgnoreCase) < 0;
            var hasQuestionCountMismatch = currentQuestions.Count != defaultQuestions.Count;
            var hasQuestionTextMismatch = !HaveSameQuestionTextSet(currentQuestions, defaultQuestions);
            var hasQuestionStructureMismatch = !HaveSameQuestionFingerprintSet(currentQuestions, defaultQuestions);
            var shouldRefreshMetadata = ShouldRefreshBuiltInMetadata(current, defaultTest, hasOutdatedReadableVersion);

            var needsUpgrade =
                hasQuestionCountMismatch ||
                hasQuestionTextMismatch ||
                hasQuestionStructureMismatch ||
                shouldRefreshMetadata;

            if (!needsUpgrade)
                continue;

            if (shouldRefreshMetadata)
            {
                current.Title = defaultTest.Title;
                current.Description = defaultTest.Description;
                changed = true;
            }

            if (hasQuestionCountMismatch || hasQuestionTextMismatch || hasQuestionStructureMismatch)
            {
                current.Questions = defaultTest.Questions
                    .Select(CloneQuestion)
                    .ToList();
                changed = true;
            }

            if (current.TimeMinutes <= 0)
            {
                current.TimeMinutes = defaultTest.TimeMinutes;
                changed = true;
            }
        }

        return changed;
    }

    private static bool ShouldRefreshBuiltInMetadata(Test current, Test defaultTest, bool hasOutdatedReadableVersion)
    {
        var currentTitle = (current.Title ?? string.Empty).Trim();
        var currentDescription = (current.Description ?? string.Empty).Trim();
        var defaultTitle = (defaultTest.Title ?? string.Empty).Trim();
        var defaultDescription = (defaultTest.Description ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(currentTitle) || string.IsNullOrWhiteSpace(currentDescription))
            return true;

        if (LooksLikeLegacySectionDescription(currentDescription))
            return true;

        if (currentDescription.IndexOf("Версия банка:", StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        if (hasOutdatedReadableVersion &&
            string.Equals(currentTitle, defaultTitle, StringComparison.Ordinal) &&
            string.Equals(currentDescription, defaultDescription, StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static bool LooksLikeLegacySectionDescription(string description)
    {
        var text = (description ?? string.Empty).Trim();
        if (text.Length == 0)
            return true;

        return text.StartsWith("Полный набор тем из справки TRACE MODE 7 по разделу", StringComparison.Ordinal) ||
               text.StartsWith("Содержит объединенный банк теоретических вопросов по всем разделам справки TRACE MODE 7.", StringComparison.Ordinal);
    }

    private static bool RemoveLegacyBuiltInTests(List<Test> tests)
    {
        if (tests == null || tests.Count == 0)
            return false;

        var requiredIds = new HashSet<string>(PreferredOrder.Value, StringComparer.Ordinal);
        var removed = tests.RemoveAll(t => IsLegacyBuiltInTest(t, requiredIds));
        return removed > 0;
    }

    private static bool IsLegacyBuiltInTest(Test test, HashSet<string> requiredIds)
    {
        if (test == null)
            return false;

        var id = (test.Id ?? string.Empty).Trim();
        var title = (test.Title ?? string.Empty).Trim();

        if (requiredIds.Contains(id))
            return false;

        if (LegacyBuiltInIds.Any(x => string.Equals(x, id, StringComparison.Ordinal)))
            return true;

        if (LegacyBuiltInTitles.Any(x => string.Equals(x, title, StringComparison.Ordinal)))
            return true;

        if (id.StartsWith("tm7-level-", StringComparison.Ordinal))
            return true;

        if (id.StartsWith("tm7-final-", StringComparison.Ordinal) && !string.Equals(id, FinalTestId, StringComparison.Ordinal))
            return true;

        if (id.StartsWith("tm7-sec-", StringComparison.Ordinal))
            return true;

        return false;
    }

    private static bool HaveSameQuestionTextSet(List<Question> left, List<Question> right)
    {
        left = left ?? new List<Question>();
        right = right ?? new List<Question>();

        if (left.Count != right.Count)
            return false;

        var leftSet = new HashSet<string>(
            left.Select(q => NormalizeFingerprintPart(q.Text)),
            StringComparer.Ordinal
        );

        var rightSet = new HashSet<string>(
            right.Select(q => NormalizeFingerprintPart(q.Text)),
            StringComparer.Ordinal
        );

        return leftSet.SetEquals(rightSet);
    }

    private static bool HaveSameQuestionFingerprintSet(List<Question> left, List<Question> right)
    {
        left = left ?? new List<Question>();
        right = right ?? new List<Question>();

        if (left.Count != right.Count)
            return false;

        var leftSet = new HashSet<string>(
            left.Select(BuildQuestionUniqueKey),
            StringComparer.Ordinal
        );

        var rightSet = new HashSet<string>(
            right.Select(BuildQuestionUniqueKey),
            StringComparer.Ordinal
        );

        return leftSet.SetEquals(rightSet);
    }

    private static void SynchronizeFinalMixedTest(List<Test> tests)
    {
        if (tests == null || tests.Count == 0)
            return;

        var sourceIds = FinalSourceTestIds.Value;
        var sourceTests = sourceIds
            .Select(id => tests.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.Ordinal)))
            .Where(t => t != null)
            .ToList();

        if (sourceTests.Count != sourceIds.Length)
            return;

        var finalTest = tests.FirstOrDefault(t => string.Equals(t.Id, FinalTestId, StringComparison.Ordinal));
        if (finalTest == null)
        {
            finalTest = new Test
            {
                Id = FinalTestId,
                Title = "Итоговый: полный пул по всем разделам",
                Description = "Сборный итоговый тест по всем разделам справки TRACE MODE 7.",
                TimeMinutes = 20,
                Questions = new List<Question>()
            };
            tests.Add(finalTest);
        }

        finalTest.Questions = finalTest.Questions ?? new List<Question>();
        finalTest.Title = "Итоговый: полный пул по всем разделам";
        finalTest.Description = "Формируется смешиванием вопросов из всех разделов справки TRACE MODE 7. Дополнительные вопросы преподавателя сохраняются.";

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

        // Сохраняем только вручную добавленные вопросы итогового теста.
        // Legacy-автоматические вопросы (без FinalSourceKey) отфильтровываются по совпадающему отпечатку.
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
