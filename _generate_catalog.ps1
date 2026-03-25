$ErrorActionPreference = "Stop"

$helpRoot = "C:\Users\Admin\Downloads\D-FTMW_base\help"
if (!(Test-Path -LiteralPath $helpRoot)) {
    throw "Help root not found: $helpRoot"
}

$sectionMap = [ordered]@{
    "00_intro" = "Программный комплекс TRACE MODE 7"
    "02_proj"  = "Разработка проекта в ИС"
    "03_chan"  = "Каналы"
    "04_sys"   = "Системные переменные TRACE MODE"
    "05_users" = "Разграничение доступа (пользователи)"
    "06_dcs"   = "Распределенные АСУ"
    "07_hw"    = "Источники / приемники TRACE MODE"
    "08_debug" = "Мониторы реального времени (МРВ)"
    "09_lang"  = "Программирование алгоритмов"
    "10_graph" = "Разработка графического интерфейса"
    "11_evrep" = "События"
    "12_arch"  = "Архивирование"
    "13_doc"   = "Генерация документов"
    "14_db"    = "Обмен с базами данных"
    "15_ems"   = "EMS, система сообщений об ошибках"
    "16_app"   = "Приложения"
}

$genericDescriptions = New-Object "System.Collections.Generic.HashSet[string]" ([System.StringComparer]::OrdinalIgnoreCase)
@(
    "Зарезервировано",
    "Зарезервирован",
    "Каналы",
    "События",
    "Приложения",
    "Архивирование",
    "Генерация документов",
    "Обмен с базами данных",
    "Разработка проекта в ИС",
    "Разработка графического интерфейса",
    "Программирование алгоритмов"
) | ForEach-Object { [void]$genericDescriptions.Add($_) }

function Clean-Text([string]$value) {
    if ($null -eq $value) { return "" }

    $text = $value
    $text = [regex]::Replace($text, "<[^>]+>", " ", "IgnoreCase")
    $text = [System.Net.WebUtility]::HtmlDecode($text)
    $text = $text.Replace([char]0xA0, ' ')
    $text = [regex]::Replace($text, "\s+", " ")
    return $text.Trim()
}

function Limit-Summary([string]$value) {
    $text = Clean-Text $value
    if ([string]::IsNullOrWhiteSpace($text)) { return "" }

    $sentences = [regex]::Split($text, "(?<=[\.\!\?;])\s+")
    if ($sentences.Count -gt 0 -and $sentences[0].Length -ge 28 -and $sentences[0].Length -le 200) {
        return $sentences[0].Trim()
    }

    if ($text.Length -le 200) {
        return $text
    }

    return $text.Substring(0, 197).TrimEnd() + "..."
}

function Get-PageLeadText([string]$htmlPath) {
    if ([string]::IsNullOrWhiteSpace($htmlPath) -or !(Test-Path -LiteralPath $htmlPath)) {
        return ""
    }

    $raw = Get-Content -Raw -LiteralPath $htmlPath
    $raw = [regex]::Replace($raw, "<script.*?</script>", " ", "IgnoreCase,Singleline")
    $raw = [regex]::Replace($raw, "<style.*?</style>", " ", "IgnoreCase,Singleline")

    $paragraphs = [regex]::Matches($raw, "<p[^>]*>(.*?)</p>", "IgnoreCase,Singleline")
    foreach ($match in $paragraphs) {
        $candidate = Limit-Summary($match.Groups[1].Value)
        if ([string]::IsNullOrWhiteSpace($candidate)) { continue }
        if ($candidate.Length -lt 24) { continue }
        if ($candidate -match "^(См\.|Click for details)") { continue }
        if ($candidate -match "^[\.\-,:; ]+$") { continue }
        return $candidate
    }

    $titleMatch = [regex]::Match($raw, "<h[1-6][^>]*>(.*?)</h[1-6]>", "IgnoreCase,Singleline")
    if ($titleMatch.Success) {
        return Limit-Summary($titleMatch.Groups[1].Value)
    }

    return ""
}

function Build-TopicSummary([string]$titleDescription, [string]$topicName, [string]$targetHtmlPath) {
    $summary = Limit-Summary $titleDescription

    if ([string]::IsNullOrWhiteSpace($summary) -or
        [string]::Equals($summary, $topicName, [System.StringComparison]::OrdinalIgnoreCase) -or
        $genericDescriptions.Contains($summary)) {
        $summary = Get-PageLeadText $targetHtmlPath
    }

    if ([string]::IsNullOrWhiteSpace($summary) -or
        [string]::Equals($summary, $topicName, [System.StringComparison]::OrdinalIgnoreCase)) {
        $summary = "Назначение и практическое применение «$topicName» в TRACE MODE 7."
    }

    return Limit-Summary $summary
}

function Escape-CSharpString([string]$s) {
    if ($null -eq $s) { return "" }
    return $s.Replace("\", "\\").Replace("""", "\""")
}

$topicsBySection = @{}
$seenBySection = @{}

foreach ($k in $sectionMap.Keys) {
    $topicsBySection[$k] = New-Object "System.Collections.Generic.List[object]"
    $seenBySection[$k] = New-Object "System.Collections.Generic.HashSet[string]" ([System.StringComparer]::OrdinalIgnoreCase)

    $sectionDir = Join-Path $helpRoot $k
    $hhcPath = Join-Path $sectionDir ($k + ".hhc.html")
    if (!(Test-Path -LiteralPath $hhcPath)) {
        throw "Section TOC not found: $hhcPath"
    }

    $content = Get-Content -Raw -LiteralPath $hhcPath
    $linkMatches = [regex]::Matches(
        $content,
        "<li[^>]*title=""([^""]*)""[^>]*>\s*<a\s+href=['""]([^'""]+)['""][^>]*>(.*?)</a>",
        "IgnoreCase,Singleline"
    )

    foreach ($m in $linkMatches) {
        $linkText = Clean-Text $m.Groups[3].Value
        if ([string]::IsNullOrWhiteSpace($linkText)) { continue }
        if ($linkText -eq "Содержание") { continue }

        if (!$seenBySection[$k].Add($linkText)) { continue }

        $href = Clean-Text $m.Groups[2].Value
        if ([string]::IsNullOrWhiteSpace($href)) { continue }
        if ($href.ToLowerInvariant().EndsWith(".hhc.html")) { continue }

        $fullPath = [System.IO.Path]::GetFullPath((Join-Path $sectionDir $href))
        $summary = Build-TopicSummary (Clean-Text $m.Groups[1].Value) $linkText $fullPath

        $topic = [PSCustomObject]@{
            Name = $linkText
            Summary = $summary
        }

        $topicsBySection[$k].Add($topic)
    }
}

$sb = New-Object System.Text.StringBuilder
function AddLine([string]$line) { [void]$sb.AppendLine($line) }

AddLine "using System;"
AddLine "using System.Collections.Generic;"
AddLine "using System.Linq;"
AddLine ""
AddLine "public static class DefaultTestCatalog"
AddLine "{"
AddLine "    public const string ReadableCatalogVersion = ""2026-03-help-full-v2"";"
AddLine ""
AddLine "    private const int MinimumQuestionsPerSection = 20;"
AddLine ""
AddLine "    public static List<Test> Create()"
AddLine "    {"
AddLine "        var sections = BuildSections();"
AddLine "        var tests = sections"
AddLine "            .Select((section, index) => BuildSectionTest(section, sections, index + 1))"
AddLine "            .ToList();"
AddLine ""
AddLine "        var finalMixed = BuildFinalMixed(tests);"
AddLine "        tests.Add(finalMixed);"
AddLine "        return tests;"
AddLine "    }"
AddLine ""
AddLine "    private static List<SectionSeed> BuildSections()"
AddLine "    {"
AddLine "        return new List<SectionSeed>"
AddLine "        {"

foreach ($k in $sectionMap.Keys) {
    $secId = "tm7-sec-" + ($k -replace "_", "-")
    $code = ($k -split "_")[0]
    $name = $sectionMap[$k]

    AddLine "            new SectionSeed"
    AddLine "            {"
    AddLine ("                Id = ""{0}""," -f (Escape-CSharpString $secId))
    AddLine ("                Code = ""{0}""," -f (Escape-CSharpString $code))
    AddLine ("                Name = ""{0}""," -f (Escape-CSharpString $name))
    AddLine "                Topics = new List<TopicSeed>"
    AddLine "                {"
    foreach ($topic in $topicsBySection[$k]) {
        AddLine "                    new TopicSeed"
        AddLine "                    {"
        AddLine ("                        Name = ""{0}""," -f (Escape-CSharpString $topic.Name))
        AddLine ("                        Summary = ""{0}""," -f (Escape-CSharpString $topic.Summary))
        AddLine "                    },"
    }
    AddLine "                }"
    AddLine "            },"
}

AddLine "        };"
AddLine "    }"
AddLine ""
AddLine "    private static Test BuildSectionTest(SectionSeed section, List<SectionSeed> allSections, int sectionIndex)"
AddLine "    {"
AddLine "        var questions = new List<Question>();"
AddLine ""
AddLine "        for (var i = 0; i < section.Topics.Count; i++)"
AddLine "        {"
AddLine "            var topic = section.Topics[i];"
AddLine "            var options = BuildSummaryOptions(topic, section, allSections, sectionIndex * 1000 + i);"
AddLine "            questions.Add(Single("
AddLine "                BuildMeaningQuestionText(topic.Name, i),"
AddLine "                topic.Summary,"
AddLine "                options"
AddLine "            ));"
AddLine "        }"
AddLine ""
AddLine "        EnsureMinimumCoverageQuestions(questions, section, allSections);"
AddLine ""
AddLine "        return new Test"
AddLine "        {"
AddLine "            Id = section.Id,"
AddLine "            Title = string.Format(""Раздел {0}: {1}"", section.Code, section.Name),"
AddLine "            Description = ""Полный набор теоретических вопросов по разделу TRACE MODE 7. Версия банка: "" + ReadableCatalogVersion,"
AddLine "            TimeMinutes = 20,"
AddLine "            Questions = EnsureUniqueByText(questions)"
AddLine "        };"
AddLine "    }"
AddLine ""
AddLine "    private static void EnsureMinimumCoverageQuestions(List<Question> questions, SectionSeed section, List<SectionSeed> allSections)"
AddLine "    {"
AddLine "        var allTopics = allSections.SelectMany(x => x.Topics).ToList();"
AddLine "        var index = 0;"
AddLine ""
AddLine "        while (questions.Count < MinimumQuestionsPerSection && section.Topics.Count > 0)"
AddLine "        {"
AddLine "            var topic = section.Topics[index % section.Topics.Count];"
AddLine "            var options = BuildTopicNameOptions(topic, section, allTopics, index + questions.Count * 31);"
AddLine "            questions.Add(Single("
AddLine "                string.Format(""Какой элемент TRACE MODE 7 соответствует описанию: «{0}»?"", topic.Summary),"
AddLine "                topic.Name,"
AddLine "                options"
AddLine "            ));"
AddLine "            index++;"
AddLine "        }"
AddLine "    }"
AddLine ""
AddLine "    private static string BuildMeaningQuestionText(string topicName, int index)"
AddLine "    {"
AddLine "        var templates = new[]"
AddLine "        {"
AddLine "            ""Что в TRACE MODE 7 наиболее точно описывает «{0}»?"","
AddLine "            ""Для чего в TRACE MODE 7 используется «{0}»?"","
AddLine "            ""Какое утверждение верно для темы «{0}»?"","
AddLine "            ""Выберите корректное описание темы «{0}»."","
AddLine "            ""Какова основная роль «{0}» в TRACE MODE 7?"""
AddLine "        };"
AddLine ""
AddLine "        if (topicName.StartsWith(""s"", StringComparison.OrdinalIgnoreCase) &&"
AddLine "            topicName.Length == 5 &&"
AddLine "            char.IsDigit(topicName[1]))"
AddLine "        {"
AddLine "            return string.Format(""Что в TRACE MODE 7 обозначает системная переменная {0}?"", topicName);"
AddLine "        }"
AddLine ""
AddLine "        if (topicName.StartsWith(""Атрибут"", StringComparison.OrdinalIgnoreCase))"
AddLine "        {"
AddLine "            return string.Format(""Что в TRACE MODE 7 характеризует «{0}»?"", topicName);"
AddLine "        }"
AddLine ""
AddLine "        var template = templates[Math.Abs(index) % templates.Length];"
AddLine "        return string.Format(template, topicName);"
AddLine "    }"
AddLine ""
AddLine "    private static string[] BuildSummaryOptions(TopicSeed correct, SectionSeed section, List<SectionSeed> allSections, int seed)"
AddLine "    {"
AddLine "        var options = new List<string> { correct.Summary };"
AddLine ""
AddLine "        var sameSection = section.Topics"
AddLine "            .Where(t => !string.Equals(t.Name, correct.Name, StringComparison.Ordinal))"
AddLine "            .Select(t => t.Summary)"
AddLine "            .Where(x => !string.IsNullOrWhiteSpace(x))"
AddLine "            .Distinct(StringComparer.Ordinal)"
AddLine "            .ToList();"
AddLine ""
AddLine "        AddOptionsBySeed(options, sameSection, seed * 17 + 3, 1);"
AddLine ""
AddLine "        var global = allSections"
AddLine "            .SelectMany(s => s.Topics)"
AddLine "            .Where(t => !string.Equals(t.Name, correct.Name, StringComparison.Ordinal))"
AddLine "            .Select(t => t.Summary)"
AddLine "            .Where(x => !string.IsNullOrWhiteSpace(x))"
AddLine "            .Distinct(StringComparer.Ordinal)"
AddLine "            .ToList();"
AddLine ""
AddLine "        AddOptionsBySeed(options, global, seed * 29 + 7, 4);"
AddLine ""
AddLine "        if (options.Count < 4)"
AddLine "        {"
AddLine "            options.Add(""Относится к базовой настройке графических элементов без обмена данными."");"
AddLine "            options.Add(""Используется только для лицензирования и не влияет на логику проекта."");"
AddLine "            options.Add(""Определяет исключительно цветовую схему интерфейса без функциональной роли."");"
AddLine "        }"
AddLine ""
AddLine "        return Shuffle(seed * 43 + 11, options).Take(4).ToArray();"
AddLine "    }"
AddLine ""
AddLine "    private static string[] BuildTopicNameOptions(TopicSeed correct, SectionSeed section, List<TopicSeed> allTopics, int seed)"
AddLine "    {"
AddLine "        var options = new List<string> { correct.Name };"
AddLine ""
AddLine "        var sameSection = section.Topics"
AddLine "            .Where(t => !string.Equals(t.Name, correct.Name, StringComparison.Ordinal))"
AddLine "            .Select(t => t.Name)"
AddLine "            .Distinct(StringComparer.Ordinal)"
AddLine "            .ToList();"
AddLine ""
AddLine "        AddOptionsBySeed(options, sameSection, seed * 13 + 5, 2);"
AddLine ""
AddLine "        var global = allTopics"
AddLine "            .Where(t => !string.Equals(t.Name, correct.Name, StringComparison.Ordinal))"
AddLine "            .Select(t => t.Name)"
AddLine "            .Distinct(StringComparer.Ordinal)"
AddLine "            .ToList();"
AddLine ""
AddLine "        AddOptionsBySeed(options, global, seed * 31 + 9, 4);"
AddLine "        return Shuffle(seed * 61 + 17, options).Take(4).ToArray();"
AddLine "    }"
AddLine ""
AddLine "    private static void AddOptionsBySeed(List<string> target, List<string> source, int seed, int limit)"
AddLine "    {"
AddLine "        if (target == null || source == null || source.Count == 0 || limit <= 0)"
AddLine "            return;"
AddLine ""
AddLine "        var start = Math.Abs(seed) % Math.Max(1, source.Count);"
AddLine "        var added = 0;"
AddLine "        for (var i = 0; i < source.Count && added < limit; i++)"
AddLine "        {"
AddLine "            var candidate = source[(start + i) % source.Count];"
AddLine "            if (string.IsNullOrWhiteSpace(candidate))"
AddLine "                continue;"
AddLine ""
AddLine "            if (target.Any(x => string.Equals(x, candidate, StringComparison.Ordinal)))"
AddLine "                continue;"
AddLine ""
AddLine "            target.Add(candidate);"
AddLine "            added++;"
AddLine "        }"
AddLine "    }"
AddLine ""
AddLine "    private static Test BuildFinalMixed(List<Test> sourceTests)"
AddLine "    {"
AddLine "        var mixed = sourceTests"
AddLine "            .SelectMany(t => t.Questions ?? new List<Question>())"
AddLine "            .Select(CloneQuestion)"
AddLine "            .ToList();"
AddLine ""
AddLine "        return new Test"
AddLine "        {"
AddLine "            Id = ""tm7-final-advanced"","
AddLine "            Title = ""Итоговый: полный пул по всем разделам"","
AddLine "            Description = ""Содержит объединенный банк теоретических вопросов по всем разделам справки TRACE MODE 7."","
AddLine "            TimeMinutes = 20,"
AddLine "            Questions = EnsureUniqueByText(mixed)"
AddLine "        };"
AddLine "    }"
AddLine ""
AddLine "    private static List<Question> EnsureUniqueByText(IEnumerable<Question> questions)"
AddLine "    {"
AddLine "        var result = new List<Question>();"
AddLine "        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);"
AddLine ""
AddLine "        foreach (var q in questions ?? Enumerable.Empty<Question>())"
AddLine "        {"
AddLine "            var text = (q.Text ?? string.Empty).Trim();"
AddLine "            if (string.IsNullOrWhiteSpace(text))"
AddLine "                continue;"
AddLine ""
AddLine "            if (!seen.Add(text))"
AddLine "                continue;"
AddLine ""
AddLine "            result.Add(q);"
AddLine "        }"
AddLine ""
AddLine "        return result;"
AddLine "    }"
AddLine ""
AddLine "    private static Question CloneQuestion(Question source)"
AddLine "    {"
AddLine "        return new Question"
AddLine "        {"
AddLine "            Id = source.Id,"
AddLine "            Text = source.Text,"
AddLine "            Type = source.Type,"
AddLine "            Options = source.Options != null ? new List<string>(source.Options) : new List<string>(),"
AddLine "            Answer = source.Answer,"
AddLine "            AnswerEncrypted = source.AnswerEncrypted,"
AddLine "            FinalSourceKey = source.FinalSourceKey,"
AddLine "            AnswerHash = source.AnswerHash,"
AddLine "            AnswerSalt = source.AnswerSalt"
AddLine "        };"
AddLine "    }"
AddLine ""
AddLine "    private static Question Single(string text, string answer, params string[] options)"
AddLine "    {"
AddLine "        return new Question"
AddLine "        {"
AddLine "            Id = Guid.NewGuid().ToString(),"
AddLine "            Text = text,"
AddLine "            Type = QuestionType.Single,"
AddLine "            Options = options.Distinct(StringComparer.Ordinal).ToList(),"
AddLine "            Answer = answer"
AddLine "        };"
AddLine "    }"
AddLine ""
AddLine "    private static string[] Shuffle(int seed, IEnumerable<string> options)"
AddLine "    {"
AddLine "        var rnd = new Random(seed * 97 + 13);"
AddLine "        return (options ?? Enumerable.Empty<string>())"
AddLine "            .Distinct(StringComparer.Ordinal)"
AddLine "            .OrderBy(_ => rnd.Next())"
AddLine "            .ToArray();"
AddLine "    }"
AddLine ""
AddLine "    private sealed class SectionSeed"
AddLine "    {"
AddLine "        public string Id { get; set; }"
AddLine "        public string Code { get; set; }"
AddLine "        public string Name { get; set; }"
AddLine "        public List<TopicSeed> Topics { get; set; } = new List<TopicSeed>();"
AddLine "    }"
AddLine ""
AddLine "    private sealed class TopicSeed"
AddLine "    {"
AddLine "        public string Name { get; set; }"
AddLine "        public string Summary { get; set; }"
AddLine "    }"
AddLine "}"

[System.IO.File]::WriteAllText(
    (Resolve-Path ".\DefaultTestCatalog.cs"),
    $sb.ToString(),
    (New-Object System.Text.UTF8Encoding($true))
)

"Generated DefaultTestCatalog.cs with knowledge-based questions."
foreach ($k in $sectionMap.Keys) {
    " - ${k}: $($topicsBySection[$k].Count) topics"
}
