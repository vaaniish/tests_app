using System;
using System.Collections.Generic;
using System.Linq;

public static class DefaultTestCatalog
{
    public const string ReadableCatalogVersion = "2026-03-theory-v1";

    private const int PoolSize = 50;
    private const int SinglePerTest = 30;
    private const int MultiPerTest = 12;
    private const int TextPerTest = 8;

    public static List<Test> Create()
    {
        var level1 = BuildLevel1();
        var level2 = BuildLevel2();
        var level3 = BuildLevel3();
        var level45 = BuildLevel45();
        var finalMixed = BuildFinalMixed(level1, level2, level3, level45);

        return new List<Test>
        {
            level1,
            level2,
            level3,
            level45,
            finalMixed
        };
    }

    private static Test BuildLevel1()
    {
        var concepts = new[]
        {
            Concept("ANALOG-канал", "Канал для непрерывных (вещественных) значений"),
            Concept("CALL-канал", "Канал для вызова шаблона и передачи аргументов"),
            Concept("Аргумент IN", "Входной параметр, который шаблон получает извне"),
            Concept("Аргумент OUT", "Выходной параметр, который шаблон отдает наружу"),
            Concept("Шаблон", "Повторно используемый блок логики и интерфейса"),
            Concept("Источник канала", "Объект, откуда канал берет исходные данные"),
            Concept("Масштабирование", "Преобразование сырого сигнала в инженерные единицы"),
            Concept("Единица измерения", "Текстовое обозначение физической величины"),
            Concept("Физическая величина", "Тип измеряемого параметра: температура, давление и т.п."),
            Concept("«Записать проект»", "Сохранение изменений в файле проекта"),
            Concept("«Сохранить для МРВ»", "Подготовка проекта к запуску в исполнительном контуре"),
            Concept("МРВ", "Исполнительная среда (runtime) проекта TM7")
        };

        var prompts = new[]
        {
            "Что в TM7 означает термин",
            "Как правильно понимать в TM7 понятие",
            "Выберите корректное определение термина",
            "Что из вариантов точнее всего описывает"
        };

        var multi = new[]
        {
            MultiStd(
                "Что относится к базовой структуре проекта TM7?",
                new[]
                {
                    "Каналы",
                    "Шаблоны",
                    "Аргументы"
                },
                "Случайные css-стили",
                "Таблица браузерных cookie"),
            MultiStd(
                "Что важно для корректной передачи значения через CALL?",
                new[]
                {
                    "Совпадение имен аргументов",
                    "Совместимые типы аргументов",
                    "Понимание направления IN/OUT"
                },
                "Цвет формы",
                "Размер иконки"),
            MultiStd(
                "Что обычно входит в минимальный цикл подготовки проекта к запуску?",
                new[]
                {
                    "Проверка конфигурации",
                    "Записать проект",
                    "Сохранить для МРВ"
                },
                "Удалить все вопросы",
                "Выключить журнал событий")
        };

        var text = new[]
        {
            Text("Канал для непрерывного значения (одно слово):", "analog"),
            Text("Канал вызова шаблона (одно слово):", "call"),
            Text("Исполнительный контур TM7 (одно слово):", "мрв"),
            Text("Входной аргумент шаблона (одно слово):", "in"),
            Text("Выходной аргумент шаблона (одно слово):", "out"),
            Text("Преобразование сигнала в инженерную шкалу (одно слово):", "масштабирование"),
            Text("Команда перед выгрузкой в runtime: сохранить для ... (одно слово):", "мрв"),
            Text("Главный объект переиспользуемой логики в TM7 (одно слово):", "шаблон"),
            Text("Тип данных для вещественного значения (одно слово):", "real"),
            Text("Параметр, откуда канал читает исходные данные (одно слово):", "источник")
        };

        return BuildTheoryTest(
            "tm7-level-1",
            "Уровень 1: Базовые понятия TRACE MODE 7",
            "Теоретический банк по каналам, аргументам и структуре проекта. Версия банка: " + ReadableCatalogVersion,
            20,
            "L1",
            concepts,
            prompts,
            multi,
            text
        );
    }

    private static Test BuildLevel2()
    {
        var concepts = new[]
        {
            Concept("СПАД (SIAD)", "Подсистема хранения истории значений"),
            Concept("Событие", "Зафиксированное системой состояние с меткой времени"),
            Concept("Тревога", "Событие, требующее внимания оператора"),
            Concept("Приоритет тревоги", "Степень важности тревожного сообщения"),
            Concept("Deadband", "Зона нечувствительности около порога"),
            Concept("Гистерезис", "Разница порогов включения и возврата"),
            Concept("Журнал событий", "Упорядоченный список событий во времени"),
            Concept("Метка времени", "Точное время регистрации записи"),
            Concept("Фильтр событий", "Отбор сообщений по заданным условиям"),
            Concept("Период архивации", "Интервал записи значения в историю"),
            Concept("Срок хранения", "Время, в течение которого история сохраняется"),
            Concept("Backup архива", "Резервная копия исторических данных")
        };

        var prompts = new[]
        {
            "Что означает термин",
            "Выберите правильное определение",
            "Как в контексте СПАД понимают понятие",
            "Что из перечисленного ближе по смыслу к"
        };

        var multi = new[]
        {
            MultiStd(
                "Что обязательно для качественной записи события?",
                new[]
                {
                    "Метка времени",
                    "Идентификатор канала",
                    "Тип/класс события"
                },
                "Шрифт заголовка окна",
                "Цвет обоев рабочего стола"),
            MultiStd(
                "Что помогает уменьшить поток ложных тревог?",
                new[]
                {
                    "Корректные пороги",
                    "Deadband/гистерезис",
                    "Приоритизация сообщений"
                },
                "Отключение истории",
                "Удаление шкал трендов"),
            MultiStd(
                "Что повышает надежность исторических данных?",
                new[]
                {
                    "Резервные копии",
                    "Проверка восстановления",
                    "Понятная политика хранения"
                },
                "Одна копия на том же диске",
                "Отключение журнала событий")
        };

        var text = new[]
        {
            Text("Аббревиатура архива истории в TM7 (одно слово):", "спад"),
            Text("Зона нечувствительности около порога (одно слово):", "deadband"),
            Text("Степень важности тревоги (одно слово):", "приоритет"),
            Text("Точное время записи события (одно слово):", "время"),
            Text("Список событий в порядке времени (одно слово):", "журнал"),
            Text("Копия архива на случай потери данных (одно слово):", "backup"),
            Text("Отбор сообщений по условию (одно слово):", "фильтр"),
            Text("Подсистема хранения истории (одно слово):", "спад"),
            Text("Интервал записи значения в архив (одно слово):", "период"),
            Text("Параметр для уменьшения дребезга тревог (одно слово):", "гистерезис")
        };

        return BuildTheoryTest(
            "tm7-level-2",
            "Уровень 2: СПАД (SIAD)-архив и события",
            "Теоретический банк по истории, тревогам и журналу событий. Версия банка: " + ReadableCatalogVersion,
            20,
            "L2",
            concepts,
            prompts,
            multi,
            text
        );
    }

    private static Test BuildLevel3()
    {
        var concepts = new[]
        {
            Concept("Modbus", "Промышленный протокол обмена между устройствами"),
            Concept("0x01", "Функция чтения дискретных состояний"),
            Concept("0x0F", "Функция записи нескольких coil"),
            Concept("Coil", "Битовая точка дискретного управления"),
            Concept("Register", "Регистр для хранения числовых значений"),
            Concept("STATE", "Канал фактического состояния оборудования"),
            Concept("mCMD", "Канал управляющей команды"),
            Concept("Scheduler", "Механизм расписания изменения заданий"),
            Concept("Manual", "Ручной режим управления"),
            Concept("Auto", "Автоматический режим управления"),
            Concept("DPA-связка", "Логическая пара команда/состояние"),
            Concept("Обратная связь", "Подтверждение фактического выполнения команды")
        };

        var prompts = new[]
        {
            "Что в теории TM7 означает",
            "Выберите точное определение",
            "Как правильно трактуется термин",
            "Что ближе всего к смыслу понятия"
        };

        var multi = new[]
        {
            MultiStd(
                "Что проверяют теоретически перед переходом в Auto?",
                new[]
                {
                    "Есть канал команды",
                    "Есть канал состояния",
                    "Есть источник задания (например Scheduler)"
                },
                "Размер окна оператора",
                "Цвет ползунка"),
            MultiStd(
                "Что относится к логике DPA-связки?",
                new[]
                {
                    "Разделение команды и факта",
                    "Контроль подтверждения состояния",
                    "Согласование режимов Manual/Auto"
                },
                "Случайный порядок полей",
                "Смена темы интерфейса"),
            MultiStd(
                "Что относится к базовым понятиям Modbus?",
                new[]
                {
                    "Функции протокола",
                    "Адреса точек",
                    "Типы данных каналов"
                },
                "Markdown-разметка",
                "CSS-переменные")
        };

        var text = new[]
        {
            Text("Промышленный протокол уровня 3 (одно слово):", "modbus"),
            Text("Функция чтения дискретных состояний (одно слово):", "0x01"),
            Text("Функция записи нескольких coil (одно слово):", "0x0f"),
            Text("Канал факта в DPA-связке (одно слово):", "state"),
            Text("Канал команды в DPA-связке (одно слово):", "mcmd"),
            Text("Планировщик смены заданий (одно слово):", "scheduler"),
            Text("Ручной режим управления (одно слово):", "manual"),
            Text("Автоматический режим управления (одно слово):", "auto"),
            Text("Подтверждение реального выполнения команды (2 слова):", "обратная связь"),
            Text("Битовая точка дискретного управления (одно слово):", "coil")
        };

        return BuildTheoryTest(
            "tm7-level-3",
            "Уровень 3: MODBUS, DPA и режимы управления",
            "Теоретический банк по протоколу Modbus, DPA и режимам управления. Версия банка: " + ReadableCatalogVersion,
            20,
            "L3",
            concepts,
            prompts,
            multi,
            text
        );
    }

    private static Test BuildLevel45()
    {
        var concepts = new[]
        {
            Concept("Шаблон отчета", "Структура документа с фиксированными полями"),
            Concept("Источник отчета", "Набор каналов и период, из которых берутся данные"),
            Concept("Web-доступ", "Работа с системой через браузерный интерфейс"),
            Concept("Роль пользователя", "Набор разрешенных действий в системе"),
            Concept("Least privilege", "Принцип минимально необходимых прав"),
            Concept("Audit trail", "Журнал действий пользователей"),
            Concept("Серверная валидация", "Проверка допустимости данных на сервере"),
            Concept("ISA/IEC 62443", "Стандарт ИБ для промышленных систем"),
            Concept("ISA-95", "Модель интеграции уровней предприятия"),
            Concept("Сегментация сети", "Разделение сети на зоны с разными правилами доступа"),
            Concept("Критическое действие", "Операция, влияющая на безопасность или технологию"),
            Concept("Журналирование", "Фиксация действий и изменений для анализа")
        };

        var prompts = new[]
        {
            "Что означает термин",
            "Как в теории правильно понимать понятие",
            "Выберите корректное определение",
            "Что точнее описывает"
        };

        var multi = new[]
        {
            MultiStd(
                "Что относится к базовым мерам ИБ для web-доступа в АСУТП?",
                new[]
                {
                    "Ролевой доступ",
                    "Сегментация сети",
                    "Журналирование действий"
                },
                "Один общий пароль для всех",
                "Отключение аудита"),
            MultiStd(
                "Что делает отчет инженерно полезным?",
                new[]
                {
                    "Понятный источник данных",
                    "Единый шаблон документа",
                    "Проверяемые правила заполнения"
                },
                "Случайные названия полей",
                "Ручной ввод из памяти"),
            MultiStd(
                "Что повышает безопасность изменения уставок через web?",
                new[]
                {
                    "Проверка диапазонов",
                    "Подтверждение критичных действий",
                    "Логирование изменений"
                },
                "Отключение авторизации",
                "Полный запрет протоколирования")
        };

        var text = new[]
        {
            Text("Стандарт ИБ для промышленных систем (одно слово):", "62443"),
            Text("Модель уровней интеграции предприятия (одно слово):", "isa-95"),
            Text("Принцип минимальных прав (2 слова):", "least privilege"),
            Text("Журнал действий пользователя (2 слова):", "audit trail"),
            Text("Проверка данных на стороне сервера (одно слово):", "валидация"),
            Text("Разделение сети на зоны (одно слово):", "сегментация"),
            Text("Набор прав пользователя в системе (одно слово):", "роль"),
            Text("Структура документа с полями (одно слово):", "шаблон"),
            Text("Фиксация изменений и действий (одно слово):", "журналирование"),
            Text("Работа с системой через браузер (одно слово):", "web")
        };

        return BuildTheoryTest(
            "tm7-level-4-5",
            "Уровни 4-5: Документы, Web-доступ и ИБ АСУТП",
            "Теоретический банк по отчетам, web-доступу и безопасности. Версия банка: " + ReadableCatalogVersion,
            20,
            "L45",
            concepts,
            prompts,
            multi,
            text
        );
    }

    private static Test BuildFinalMixed(Test level1, Test level2, Test level3, Test level45)
    {
        var mixed = new List<Question>();
        var sourceTests = new[] { level1, level2, level3, level45 };

        foreach (var test in sourceTests)
        {
            foreach (var question in test.Questions ?? new List<Question>())
            {
                mixed.Add(CloneQuestion(question));
            }
        }

        return new Test
        {
            Id = "tm7-final-advanced",
            Title = "Итоговый: смешанный по уровням 1-4",
            Description = "Формируется смешиванием вопросов из уровней 1, 2, 3 и 4-5. Дополнительные вопросы преподавателя сохраняются.",
            TimeMinutes = 20,
            Questions = mixed
        };
    }

    private static Test BuildTheoryTest(
        string id,
        string title,
        string description,
        int timeMinutes,
        string tag,
        TheoryConcept[] concepts,
        string[] prompts,
        StandardMulti[] multi,
        Question[] textQuestions)
    {
        var questions = new List<Question>();
        questions.AddRange(BuildTheorySingles(tag, concepts, prompts));

        for (var i = 0; i < MultiPerTest; i++)
        {
            var std = multi[i % multi.Length];
            var options = Shuffle(2000 + i + tag.Length, std.AllOptions());
            questions.Add(Multiple(
                $"[{tag} контроль {i + 1}] {std.Stem}",
                std.Correct,
                options
            ));
        }

        questions.AddRange(BuildTextPool(textQuestions).Take(TextPerTest));
        questions = EnsureUniqueByText(questions);

        return new Test
        {
            Id = id,
            Title = title,
            Description = description,
            TimeMinutes = timeMinutes,
            Questions = questions.Take(PoolSize).ToList()
        };
    }

    private static IEnumerable<Question> BuildTheorySingles(string tag, TheoryConcept[] concepts, string[] prompts)
    {
        var list = new List<Question>();
        var definitionBank = concepts.Select(x => x.Definition).Distinct(StringComparer.Ordinal).ToArray();

        for (var i = 0; i < SinglePerTest; i++)
        {
            var concept = concepts[i % concepts.Length];
            var prompt = prompts[(i / concepts.Length) % prompts.Length];
            var options = BuildOptions(definitionBank, concept.Definition, 100 + i + tag.Length);

            list.Add(Single(
                $"[{tag} теория {i + 1}] {prompt} «{concept.Term}»?",
                concept.Definition,
                options
            ));
        }

        return list;
    }

    private static List<Question> EnsureUniqueByText(List<Question> questions)
    {
        var unique = new List<Question>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var q in questions ?? new List<Question>())
        {
            var text = (q.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            if (!seen.Add(text))
                continue;

            unique.Add(q);
        }

        return unique;
    }

    private static IEnumerable<Question> BuildTextPool(Question[] baseQuestions)
    {
        return (baseQuestions ?? new Question[0])
            .GroupBy(q => q.Text ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First());
    }

    private static TheoryConcept Concept(string term, string definition)
    {
        return new TheoryConcept(term, definition);
    }

    private static string[] BuildOptions(string[] pool, string correct, int seed)
    {
        var distractors = pool
            .Where(x => !Same(x, correct))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();

        var start = seed % Math.Max(1, distractors.Count);
        var picked = new List<string> { correct };

        for (var i = 0; i < 3 && distractors.Count > 0; i++)
        {
            picked.Add(distractors[(start + i) % distractors.Count]);
        }

        return Shuffle(seed, picked.ToArray());
    }

    private static string[] Shuffle(int seed, string[] options)
    {
        var rnd = new Random(seed * 97 + 13);
        return options
            .Distinct(StringComparer.Ordinal)
            .OrderBy(_ => rnd.Next())
            .ToArray();
    }

    private static bool Same(string left, string right)
    {
        return string.Equals(left, right, StringComparison.Ordinal);
    }

    private static StandardMulti MultiStd(string stem, string[] correct, string d1, string d2)
    {
        return new StandardMulti(stem, correct, d1, d2);
    }

    private static Question Single(string text, string answer, params string[] options)
    {
        return new Question
        {
            Id = Guid.NewGuid().ToString(),
            Text = text,
            Type = QuestionType.Single,
            Options = options.ToList(),
            Answer = answer
        };
    }

    private static Question Multiple(string text, string[] answers, params string[] options)
    {
        return new Question
        {
            Id = Guid.NewGuid().ToString(),
            Text = text,
            Type = QuestionType.Multiple,
            Options = options.ToList(),
            Answer = string.Join(";", answers)
        };
    }

    private static Question Text(string text, string answer)
    {
        var normalizedAnswer = (answer ?? string.Empty).Trim();
        var withWordRule = EnsureTextQuestionWordRule(text, normalizedAnswer);

        return new Question
        {
            Id = Guid.NewGuid().ToString(),
            Text = withWordRule,
            Type = QuestionType.Text,
            Options = new List<string>(),
            Answer = normalizedAnswer
        };
    }

    private static string EnsureTextQuestionWordRule(string questionText, string answer)
    {
        var text = (questionText ?? string.Empty).Trim();
        var words = CountWords(answer);

        if (words <= 1)
            return text;

        if (text.IndexOf("слов", StringComparison.OrdinalIgnoreCase) >= 0 ||
            text.IndexOf("слова", StringComparison.OrdinalIgnoreCase) >= 0 ||
            text.IndexOf("слово", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return text;
        }

        return text + " (ответ: " + words + " слова)";
    }

    private static int CountWords(string value)
    {
        return (value ?? string.Empty)
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    private static Question CloneQuestion(Question source)
    {
        return new Question
        {
            Id = Guid.NewGuid().ToString(),
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

    private sealed class TheoryConcept
    {
        public TheoryConcept(string term, string definition)
        {
            Term = term;
            Definition = definition;
        }

        public string Term { get; private set; }
        public string Definition { get; private set; }
    }

    private sealed class StandardMulti
    {
        public StandardMulti(string stem, string[] correct, string d1, string d2)
        {
            Stem = stem;
            Correct = correct;
            Distractor1 = d1;
            Distractor2 = d2;
        }

        public string Stem { get; private set; }
        public string[] Correct { get; private set; }
        public string Distractor1 { get; private set; }
        public string Distractor2 { get; private set; }

        public string[] AllOptions()
        {
            return Correct.Concat(new[] { Distractor1, Distractor2 }).ToArray();
        }
    }
}
