using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class DefaultTestCatalog
{
    public const string ReadableCatalogVersion = "2026-03-help-exam-v7";

    private const int MinimumQuestionsPerTheme = 50;
    private const int DefaultThemeTimeMinutes = 20;

    public static List<Test> Create()
    {
        var sections = BuildSections();
        var themes = BuildThemes();
        var tests = themes
            .Select((theme, index) => BuildThemeTest(theme, sections, index + 1))
            .ToList();

        var finalMixed = BuildFinalMixed(tests);
        tests.Add(finalMixed);
        return tests;
    }

    private static List<SectionSeed> BuildSections()
    {
        return new List<SectionSeed>
        {
            new SectionSeed
            {
                Id = "tm7-sec-00-intro",
                Code = "00",
                Name = "Программный комплекс TRACE MODE 7",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Программный комплекс TRACE MODE 7",
                        Summary = "Справочная система TRACE MODE",
                    },
                    new TopicSeed
                    {
                        Name = "Модификации ИС TRACE MODE (разработка)",
                        Summary = "ИС выпускается в следующих форматах:",
                    },
                    new TopicSeed
                    {
                        Name = "Модификации мониторов (исполнение)",
                        Summary = "МРВ выпускаются только в профессиональном формате.",
                    },
                    new TopicSeed
                    {
                        Name = "Основные характеристики TRACE MODE 7",
                        Summary = "Обеспечение работы распределенных АСУ",
                    },
                    new TopicSeed
                    {
                        Name = "Системные требования для работы ИС и МРВ",
                        Summary = "Подробно системные требования на продукты указаны на сайте (см., например, https://www.tracemode.ru/products/D/TMW ).",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-02-proj",
                Code = "02",
                Name = "Разработка проекта в ИС",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Разработка проекта в ИС",
                        Summary = "Принцип работы монитора. Канал TRACE MODE",
                    },
                    new TopicSeed
                    {
                        Name = "Классификация объектов структуры проекта",
                        Summary = "Классификация компонентов",
                    },
                    new TopicSeed
                    {
                        Name = "Операции в ИС",
                        Summary = "Меню 'Файл' и главная панель инструментов ИС",
                    },
                    new TopicSeed
                    {
                        Name = "Задание общих настроек ИС",
                        Summary = "Для задания общих настроек ИС предназначен диалог, который открывается по команде ИС Настройки ИС :",
                    },
                    new TopicSeed
                    {
                        Name = "Сохранение проекта",
                        Summary = "Сохранение проекта для редактирования",
                    },
                    new TopicSeed
                    {
                        Name = "Отладка проекта",
                        Summary = "Для отладки проекта ИС снабжена следующими механизмами:",
                    },
                    new TopicSeed
                    {
                        Name = "Редактирование структуры проекта",
                        Summary = "Меню и главная панель инструментов навигатора проекта",
                    },
                    new TopicSeed
                    {
                        Name = "Создание объектов структуры",
                        Summary = "Для создания объектов структуры (компонентов и групп компонентов) используются типовые команды меню Проект , контекстного меню и панели инструментов навигатора (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Имена и идентификаторы объектов структуры",
                        Summary = "В качестве имени объекта структуры можно использовать произвольное строковое выражение.",
                    },
                    new TopicSeed
                    {
                        Name = "Перемещение объектов структуры",
                        Summary = "Операция перемещения включает два действия – удаление объекта из места его начального расположения и вставку в указанную группу (слой).",
                    },
                    new TopicSeed
                    {
                        Name = "Копирование и вставка объекта структуры",
                        Summary = "Копирование объекта в буфер обмена",
                    },
                    new TopicSeed
                    {
                        Name = "Отображение свойств объектов структуры",
                        Summary = "Для отображения свойств объекта структуры проекта используются:",
                    },
                    new TopicSeed
                    {
                        Name = "Групповое редактирование",
                        Summary = "Группа каналов любого узла (кроме узла NetLink) имеет свой табличный редактор (параметры этого редактора задаются в диалоге настроек среды – см.",
                    },
                    new TopicSeed
                    {
                        Name = "Окно свойств объекта структуры проекта",
                        Summary = "По команде Свойства (см. Меню и главная панель инструментов навигатора проекта ) в нижней части ИС открывается окно свойств выделенного объекта структуры проекта.",
                    },
                    new TopicSeed
                    {
                        Name = "Задание параметров узла",
                        Summary = "Вкладка 'Основные' редактора узла",
                    },
                    new TopicSeed
                    {
                        Name = "Редактор параметров COM-порта",
                        Summary = "Если в имени COM-порта встречаются символы / \\ % u .",
                    },
                    new TopicSeed
                    {
                        Name = "Табличный редактор аргументов",
                        Summary = "Разновидности редактора аргументов",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-03-chan",
                Code = "03",
                Name = "Каналы",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Каналы",
                        Summary = "Загрузка ядер процессора",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты каналов",
                        Summary = "Замечания о передаче значения",
                    },
                    new TopicSeed
                    {
                        Name = "Указатель атрибутов (по номеру)",
                        Summary = "Данный раздел содержит список ссылок на описание атрибутов по их номеру.",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 48, 56, 78, 118, 126",
                        Summary = "Информация о канале (класс, число бит и т.п.)",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 79-80, 82-83, 117, 127",
                        Summary = "Текстовые атрибуты (кодировка, имя и т.п.)",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 5, 38",
                        Summary = "Пересчет канала",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 0-2, 9",
                        Summary = "Числовые значения канала (реальное, аппаратное, входное, выходное)",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 52, 61, 123",
                        Summary = "Качество значения",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 10-25, 62-77, 100-115",
                        Summary = "Канал DISCRETE: биты реального значения",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 26-37, 53, 85",
                        Summary = "Канал ANALOG: Границы, обработка в канале",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 45, 50, 54-55, 59, 87-88, 119, 252",
                        Summary = "Временные атрибуты (время изменения, время начала/конца данных и т.п.)",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 3, 6-8, 39-40, 81, 125, 245",
                        Summary = "Состояние, конфигурация, управление (основные атрибуты)",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 86, 124, 241",
                        Summary = "Привязки, аргументы",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 128-129",
                        Summary = "Аргументы канала CALL: запись в файл и чтение из файла",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 41-44, 58, 60",
                        Summary = "Дамп, архивирование, генерация сообщений для отчета событий и публикация",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 47",
                        Summary = "Последний сгенерированный файл",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 49",
                        Summary = "Отладка",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 51",
                        Summary = "Смена типа канала OUTPUT",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 57",
                        Summary = "Захват канала",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 84, 134-138, 154, 236-238",
                        Summary = "Режим/статус оборудования/канала.",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 89-98",
                        Summary = "Удаленный адрес",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 99, 156, 187, 235",
                        Summary = "Посылка команд управления в канал (открыть/закрыть, включить/выключить, сброс, аварийный останов )",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 120",
                        Summary = "Индикатор готовности данных",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 122",
                        Summary = "Квитирование последнего сообщения",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 9, 130, 150-160",
                        Summary = "Маскирование/размаскирование битов и внешних сигналов",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 4, 46, 139-140, 230-232, 239-240",
                        Summary = "Аварии, неисправности",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 131-133",
                        Summary = "Блокировки",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 233, 251",
                        Summary = "Открытие окон/файлов",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 242-243",
                        Summary = "Положение всплывающего экрана",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 244",
                        Summary = "Перезагрузка шаблонов/пользователей",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибуты 246-247",
                        Summary = "Формат строкового представления значения канала",
                    },
                    new TopicSeed
                    {
                        Name = "Атрибут 254",
                        Summary = "Назначение и практическое применение «Атрибут 254» в TRACE MODE 7.",
                    },
                    new TopicSeed
                    {
                        Name = "Задание общих атрибутов каналов в ИС",
                        Summary = "Раздел \"Общие\" редактора канала",
                    },
                    new TopicSeed
                    {
                        Name = "Восстановление атрибутов после рестарта",
                        Summary = "После рестарта МРВ значения атрибутов каналов могут быть восстановлены – см.",
                    },
                    new TopicSeed
                    {
                        Name = "Числовые каналы",
                        Summary = "Обработка данных в числовых каналах",
                    },
                    new TopicSeed
                    {
                        Name = "Канал класса ANALOG",
                        Summary = "Границы и интервалы канала ANALOG",
                    },
                    new TopicSeed
                    {
                        Name = "Канал класса DISCRETE",
                        Summary = "DISCRETE. Генерация сообщений",
                    },
                    new TopicSeed
                    {
                        Name = "CNT-канал",
                        Summary = "Каналы INPUT класса DISCRETE с Формат =3∼∼ CNT предназначены для приема 32-разрядных десятичных беззнаковых целых значений от счетчиков ICP DAS и IEC104.",
                    },
                    new TopicSeed
                    {
                        Name = "Канал класса TIME",
                        Summary = "TIME. Генерация сообщений",
                    },
                    new TopicSeed
                    {
                        Name = "DPA-канал",
                        Summary = "Для работы с DPA-элементами (выключателями, насосами и т.п.), использующими 2 бита для описания положения",
                    },
                    new TopicSeed
                    {
                        Name = "Канал класса CALL",
                        Summary = "Атрибуты канала класса CALL",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Program",
                        Summary = "Канал вызова программы",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.SQLQuery",
                        Summary = "Канал вызова связи с БД",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Document(Report)",
                        Summary = "Канал генерации документа (отчета)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Screen",
                        Summary = "Канал вызова графического экрана",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Panel",
                        Summary = "Назначение и практическое применение «Канал CALL.Panel» в TRACE MODE 7.",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.ChGroupReq",
                        Summary = "Многофункциональный канал",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.TableFunction",
                        Summary = "Табличные функции y(x)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.LongFromBits",
                        Summary = "Упаковка битов аргументов IN в реальное значение/аргумент OUT",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.BitsFromLong",
                        Summary = "Формирование значений аргументов OUT по реальному значению/аргументам IN",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL с типами вызова 12-14",
                        Summary = "Сравнение аргументов",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Sum",
                        Summary = "Различные виды суммирования",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Set",
                        Summary = "Присвоение значений",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.RT_Statistics",
                        Summary = "Статистика по значениям в реальном времени (без архива)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Latch",
                        Summary = "Фиксация событий",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.PulseOut",
                        Summary = "Генерация импульсов",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.MOVE",
                        Summary = "Присвоение значений",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Comparator",
                        Summary = "Сравнение значений аргументов",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.P_DP_Light",
                        Summary = "Связь DPA-каналов с каналами обмена",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.PID_PDD",
                        Summary = "PID- или PDD-регулятор",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.LocalSnap",
                        Summary = "Повременные срезы архива",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.SimpleProtection",
                        Summary = "Простая защита",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.LocalStatistics",
                        Summary = "Обработка данных архива по каналам",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.LocalList",
                        Summary = "Выборка данных из архива по каналам",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.WebTablePage",
                        Summary = "Назначение и практическое применение «Канал CALL.WebTablePage» в TRACE MODE 7.",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.PulseReg",
                        Summary = "Импульсный регулятор",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.SlaveGroup",
                        Summary = "Группировка каналов для Slave-протоколов",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Pack_STR",
                        Summary = "Сборка строки",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.AsyncCollection",
                        Summary = "Пошаговое исполнение рецептов (действий)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.UnPack_STR",
                        Summary = "Разбор строки",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.WebHTrendChGr",
                        Summary = "Для работы трендов в web-клиенте",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.ChannelGroup",
                        Summary = "Таблицы анализа каналов",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.SchedulerSet",
                        Summary = "Набор планировщиков CALL.Scheduler / недельное расписание",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Scheduler",
                        Summary = "Планировщик (расписание на дни заданного типа)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Recipe",
                        Summary = "Рецепты",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Exec",
                        Summary = "Запуск приложения",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.TVC",
                        Summary = "Табличная функция x(t)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.OtherProj",
                        Summary = "Запись/чтение переменных произвольного узла произвольного проекта",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.STRING",
                        Summary = "Хранение строковых данных",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Vector",
                        Summary = "Чтение и хранение однотипных данных (кроме STRING) на определенный момент времени (срез).",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.Model",
                        Summary = "Модели объекта/емкости/задвижки/клапана",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.HTML",
                        Summary = "Назначение и практическое применение «Канал CALL.HTML» в TRACE MODE 7.",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.DATA",
                        Summary = "Хранение данных (выборок)",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.EXCEPT_DAY",
                        Summary = "Корректировка рабочего календаря",
                    },
                    new TopicSeed
                    {
                        Name = "Канал CALL.EMAIL",
                        Summary = "Отправка документов/файлов/сообщений по электронной почте",
                    },
                    new TopicSeed
                    {
                        Name = "Подтипы каналов",
                        Summary = "Подтип и дополнение к подтипу являются характеристиками каналов, номера этих параметров отображаются – см.",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-04-sys",
                Code = "04",
                Name = "Системные переменные TRACE MODE",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Системные переменные TRACE MODE",
                        Summary = "Автопостроение системных переменных при перетаскивании источника",
                    },
                    new TopicSeed
                    {
                        Name = "s1400",
                        Summary = "Параметры узла",
                    },
                    new TopicSeed
                    {
                        Name = "s1401",
                        Summary = "Конфигурации (форматы) генерируемых XML-файлов",
                    },
                    new TopicSeed
                    {
                        Name = "s1402",
                        Summary = "Конфигурации (форматы) генерируемых XML-файлов",
                    },
                    new TopicSeed
                    {
                        Name = "s1403",
                        Summary = "Настройка экспорта отчета событий",
                    },
                    new TopicSeed
                    {
                        Name = "s1404",
                        Summary = "Настройка экспорта текущих данных числовых каналов",
                    },
                    new TopicSeed
                    {
                        Name = "s1405",
                        Summary = "Пересчет ФЛАГИ",
                    },
                    new TopicSeed
                    {
                        Name = "s1406",
                        Summary = "Пересчет F1-F4",
                    },
                    new TopicSeed
                    {
                        Name = "s1407",
                        Summary = "Экспорт SIAD",
                    },
                    new TopicSeed
                    {
                        Name = "s1408",
                        Summary = "SIAD: копирование, параметры",
                    },
                    new TopicSeed
                    {
                        Name = "s1409",
                        Summary = "Экспорт отчета событий",
                    },
                    new TopicSeed
                    {
                        Name = "s1410",
                        Summary = "Параметры выбранного пользователя",
                    },
                    new TopicSeed
                    {
                        Name = "s1411",
                        Summary = "Параметры текущего пользователя",
                    },
                    new TopicSeed
                    {
                        Name = "s1412",
                        Summary = "Код клавиши",
                    },
                    new TopicSeed
                    {
                        Name = "s1413",
                        Summary = "Параметры потоков",
                    },
                    new TopicSeed
                    {
                        Name = "s1414",
                        Summary = "Отладка",
                    },
                    new TopicSeed
                    {
                        Name = "s1415",
                        Summary = "Экспорт VCTR",
                    },
                    new TopicSeed
                    {
                        Name = "s1416",
                        Summary = "Запись временных меток в аргументы и атрибуты",
                    },
                    new TopicSeed
                    {
                        Name = "s1417",
                        Summary = "Временные параметры",
                    },
                    new TopicSeed
                    {
                        Name = "s1418",
                        Summary = "VCTR: копирование, параметры",
                    },
                    new TopicSeed
                    {
                        Name = "s1419",
                        Summary = "Параметры обмена по последовательным портам",
                    },
                    new TopicSeed
                    {
                        Name = "s1420",
                        Summary = "Используется для просмотра онлайн-трансляций и управления видеопотоками",
                    },
                    new TopicSeed
                    {
                        Name = "s1421",
                        Summary = "Параметры обмена по TCP/IP",
                    },
                    new TopicSeed
                    {
                        Name = "s1422",
                        Summary = "DPA не в норм.положении",
                    },
                    new TopicSeed
                    {
                        Name = "s1423",
                        Summary = "Некоторые параметры узла",
                    },
                    new TopicSeed
                    {
                        Name = "s1424",
                        Summary = "Видимость окон МРВ",
                    },
                    new TopicSeed
                    {
                        Name = "s1425",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1426",
                        Summary = "Работа с Telegram-ботом",
                    },
                    new TopicSeed
                    {
                        Name = "s1427",
                        Summary = "Корректировка времени, синхронизация",
                    },
                    new TopicSeed
                    {
                        Name = "s1428",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1429",
                        Summary = "Воспроизведение звукового файла в NetLink",
                    },
                    new TopicSeed
                    {
                        Name = "s1430",
                        Summary = "Воспроизведение звукового файла",
                    },
                    new TopicSeed
                    {
                        Name = "s1431",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1500",
                        Summary = "Диагностика SIAD",
                    },
                    new TopicSeed
                    {
                        Name = "s1501",
                        Summary = "Диагностика отчета событий",
                    },
                    new TopicSeed
                    {
                        Name = "s1502",
                        Summary = "Диагностика обмена с регистратором",
                    },
                    new TopicSeed
                    {
                        Name = "s1503",
                        Summary = "Такой переменной нет",
                    },
                    new TopicSeed
                    {
                        Name = "s1504",
                        Summary = "Код ошибки обмена по IP операционной системы",
                    },
                    new TopicSeed
                    {
                        Name = "s1505",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1506",
                        Summary = "Код ошибки обмена по протоколу DCS",
                    },
                    new TopicSeed
                    {
                        Name = "s1507",
                        Summary = "Код ошибки обмена по протоколу Modbus RTU",
                    },
                    new TopicSeed
                    {
                        Name = "s1508",
                        Summary = "Диагностика обмена через драйверы t41",
                    },
                    new TopicSeed
                    {
                        Name = "s1509",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1510",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1511",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1512",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1513",
                        Summary = "Диагностика обмена по встроенным протоколам по сети",
                    },
                    new TopicSeed
                    {
                        Name = "s1514",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1515",
                        Summary = "Диагностика обмена по OPC",
                    },
                    new TopicSeed
                    {
                        Name = "s1516",
                        Summary = "Резервные узлы",
                    },
                    new TopicSeed
                    {
                        Name = "s1517",
                        Summary = "Дамп: копирование, параметры",
                    },
                    new TopicSeed
                    {
                        Name = "s1518",
                        Summary = "Используется как шаблон канала ‘SMS’",
                    },
                    new TopicSeed
                    {
                        Name = "s1519",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1520",
                        Summary = "Ресурсы и память",
                    },
                    new TopicSeed
                    {
                        Name = "s1521",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1522",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1523",
                        Summary = "Запись произвольных строк в EvRep и другие функции",
                    },
                    new TopicSeed
                    {
                        Name = "s1524",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1525",
                        Summary = "Графика: параметры, тестирование",
                    },
                    new TopicSeed
                    {
                        Name = "s1526",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1527",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1528",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                    new TopicSeed
                    {
                        Name = "s1529",
                        Summary = "Диагностика отправки по сети",
                    },
                    new TopicSeed
                    {
                        Name = "s1530",
                        Summary = "Диагностика SIAD",
                    },
                    new TopicSeed
                    {
                        Name = "s1531",
                        Summary = "Общие замечания о системных переменных приведены в разделе Системные переменные TRACE MODE .",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-05-users",
                Code = "05",
                Name = "Разграничение доступа (пользователи)",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Разграничение доступа (пользователи)",
                        Summary = "Общие настройки пользователей в узле",
                    },
                    new TopicSeed
                    {
                        Name = "Операции в слое ПОЛЬЗОВАТЕЛИ",
                        Summary = "Редактор группы ПО УМОЛЧАНИЮ",
                    },
                    new TopicSeed
                    {
                        Name = "Операции с пользователями в узле",
                        Summary = "В узле может быть создана единственная группа Пользователи и единственная группа Внешние пользователи (автоматически не создаются).",
                    },
                    new TopicSeed
                    {
                        Name = "Разграничение доступа в МРВ",
                        Summary = "Вызов диалогов для текущего пользователя из графики",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-06-dcs",
                Code = "06",
                Name = "Распределенные АСУ",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Распределенные АСУ",
                        Summary = "Конфигурирование межкомпонентного взаимодействия",
                    },
                    new TopicSeed
                    {
                        Name = "Ссылка на канал",
                        Summary = "О создании ссылки на канал см.",
                    },
                    new TopicSeed
                    {
                        Name = "Локальная связь 'канал-канал'",
                        Summary = "Привязка в числовом канале",
                    },
                    new TopicSeed
                    {
                        Name = "Удаленная связь 'канал-канал'",
                        Summary = "Особенности связи CopyFrom",
                    },
                    new TopicSeed
                    {
                        Name = "Создание контура управления",
                        Summary = "Для создания контура управления нужно передать, в том числе с обработкой, данные из измерительного информационного потока (датчик => УСО => контроллер => операторская станция или, с точки зрения ИС...",
                    },
                    new TopicSeed
                    {
                        Name = "Резервирование в TRACE MODE",
                        Summary = "TRACE MODE поддерживает структурное резервирование узлов.",
                    },
                    new TopicSeed
                    {
                        Name = "Синхронизация резервов",
                        Summary = "Общие замечания о синхронизации",
                    },
                    new TopicSeed
                    {
                        Name = "Структурное резервирование и надежность",
                        Summary = "Структурное резервирование не всегда приводит к повышению надежности системы, поэтому до принятия решения о таком резервировании необходимо выполнить расчет надежности АСУ.",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как OPC-сервер",
                        Summary = "Для регистрации DCOM OPC сервера в Windows надо выполнить команду",
                    },
                    new TopicSeed
                    {
                        Name = "Web-клиент",
                        Summary = "Страница исторического тренда",
                    },
                    new TopicSeed
                    {
                        Name = "Telegram-бот",
                        Summary = "В состав TRACE MODE входит модуль, который обеспечивает следующий функционал бота в Telegram:",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как сервер REST API",
                        Summary = "REST API. Доступ к отчету событий",
                    },
                    new TopicSeed
                    {
                        Name = "О сертификатах",
                        Summary = "Если требуется, чтобы обмен с OPC-сервером или web-клиентом TRACE MODE производился в защищенном режиме, нужно получить соответствующие сертификаты SSL.",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-07-hw",
                Code = "07",
                Name = "Источники / приемники TRACE MODE",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Редакторы источников (приемников)",
                        Summary = "Редакторы источников/приемников отличаются по набору задаваемых в них параметров.",
                    },
                    new TopicSeed
                    {
                        Name = "Шаблоны каналов обмена",
                        Summary = "Устройство (или OPC-сервер) создается как группа в подгруппе слоя Источники (например, Источники / Протоколы Net / RS / Modbus ).",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'Контроллеры (МРВ)'",
                        Summary = "Данный механизм пока не поддерживается.",
                    },
                    new TopicSeed
                    {
                        Name = "Контроллеры XPAC, WinPac, LinPac",
                        Summary = "Модули в основном крейте",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'Платы ввода-вывода'",
                        Summary = "Данный механизм пока не поддерживается.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'PLC'",
                        Summary = "В данной группе описываются протоколы, по которым мониторы TRACE MODE могут обмениваться данными с контроллерами по RS.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по протоколу COMM2H",
                        Summary = "Данный протокол используется для обмена данными с контроллерами HITACHI, Sprecher&Schuh, Samsung.",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как OPC-клиент",
                        Summary = "Для корректной работы нужно установить OPC Core Components Redistributable соответствующей разрядности (в т.ч.",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как клиент сервера OPC DA",
                        Summary = "Группа OPC DA Сервер (создается в группе Источники .",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как клиент сервера OPC HDA",
                        Summary = "Группа OPC HDA Сервер имеет редактор, аналогичный редактору группы OPC - сервер (см.",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как клиент сервера OPC UA",
                        Summary = "Группа OPC UA DA Сервер (создается в группе Источники .",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как клиент сервера OPC UA HDA",
                        Summary = "Группа OPC UA HDA Сервер (создается в группе Источники .",
                    },
                    new TopicSeed
                    {
                        Name = "Универсальный механизм обмена с электросчетчиками",
                        Summary = "Расшифровка значений CGR.86",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'Протоколы Net/RS'",
                        Summary = "В данной группе описываются протоколы, по которым мониторы TRACE MODE могут обмениваться данными с контроллерами по RS (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по MODBUS",
                        Summary = "Групповые запросы MODBUS",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по IEC 60870-104/101",
                        Summary = "В качестве шаблонов каналов для обмена с устройствами по стандарту IEC 60870-104 или IEC 60870-101 используются шаблоны IEC104 (группа Источники .",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по SNMP",
                        Summary = "В качестве шаблонов каналов обмена используются шаблоны Источники / Протоколы Net / RS / SNMP / SNMP (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен с КР-500",
                        Summary = "В качестве шаблонов каналов TCP-обмена c КР-500 используются шаблоны Источники / Протоколы Net / RS / KR500 / KR500 (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен с контроллерами Mitsubishi",
                        Summary = "Mitsubishi. Обмен по сети",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен с контроллерами Delta",
                        Summary = "В качестве шаблонов каналов обмена с контроллерами Delta используются шаблоны Источники / Протоколы Net / RS / Delta / Delta (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен с контроллерами OMRON",
                        Summary = "В качестве шаблонов каналов обмена используются шаблоны Источники / Протоколы Net / RS / Omron / Omron (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по протоколу BACnet/IP",
                        Summary = "В качестве шаблонов каналов обмена используются шаблоны Источники / Протоколы Net / RS / BACnetIP / BACnetIP (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'UnitNET'",
                        Summary = "Команды INFO UDP_U и INFO TCP_U",
                    },
                    new TopicSeed
                    {
                        Name = "Поддержка протокола NMEA",
                        Summary = "МРВ поддерживает протокол NMEA-0183.",
                    },
                    new TopicSeed
                    {
                        Name = "Поддержка протокола MQTT",
                        Summary = "Редактор устройства ( Источники / Протоколы Net / RS / MQTT ):",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен с контроллерами OptimusDrive",
                        Summary = "В качестве шаблонов каналов обмена с контроллерами Mitsubishi используются шаблоны Источники / Протоколы Net / RS / OptimusDrive / Optimus (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по протоколу Siemens S7",
                        Summary = "Поддерживаемые ПЛК и симуляторы:",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен с контроллерами БАЗИС",
                        Summary = "В качестве шаблонов каналов обмена используются шаблоны, которые создаются в группе Источники / Протоколы Net / RS / БАЗИС (см.",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как сервер протоколов поверх TCP",
                        Summary = "МРВ может выступать в качестве сервера при обмене по различным протоколам поверх сетевых протоколов (в настоящее время поддерживается обмен по MODBUS – см.",
                    },
                    new TopicSeed
                    {
                        Name = "МРВ как сервер MODBUS TCP",
                        Summary = "Конфигурирование сервера MODBUS TCP TRACE MODE",
                    },
                    new TopicSeed
                    {
                        Name = "Встроенные генераторы TRACE MODE",
                        Summary = "Встроенные генераторы (шаблоны каналов) создаются в предопределенной группе Генераторы слоя Источники .",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'Драйвер RWH'",
                        Summary = "В данной группе настраивается обмен через драйвер rwh , dll / librwh .",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'Драйвер R/W'",
                        Summary = "В данной группе настраивается обмен через драйвер t13 ( t13 .",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'Драйвер t41/t12'",
                        Summary = "Для разработчиков TRACE MODE.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа 'UserDriver'",
                        Summary = "В данной группе настраивается обмен через пользовательский драйвер t41 (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по DCS",
                        Summary = "В качестве шаблонов каналов для обмена используются шаблоны Источники / Распределенные модули ( DCS , HART ) / I70xx / I87xxx / I70xx / I87xxx .",
                    },
                    new TopicSeed
                    {
                        Name = "Обмен по протоколу HART",
                        Summary = "Для обмена требуется библиотека t41s19 .",
                    },
                    new TopicSeed
                    {
                        Name = "Модели TRACE MODE",
                        Summary = "Модели (шаблоны каналов) создаются в предопределенной группе Источники \\ Алгоритмы \\ Модели .",
                    },
                    new TopicSeed
                    {
                        Name = "Алгоритмы управления TRACE MODE",
                        Summary = "Алгоритмы управления (шаблоны каналов) создаются в предопределенной группе Источники \\ Алгоритмы \\ Управление .",
                    },
                    new TopicSeed
                    {
                        Name = "Шаблон канала 'EMAIL'",
                        Summary = "Редактор имеет следующий вид (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Шаблон канала 'SMS'",
                        Summary = "Редактор имеет следующий вид (см.",
                    },
                    new TopicSeed
                    {
                        Name = "s1420_Cameras",
                        Summary = "Просмотр онлайн-трансляций и управление видеопотоками",
                    },
                    new TopicSeed
                    {
                        Name = "Разработка драйверов",
                        Summary = "TRACE MODE поддерживает обмен данными с наиболее распространенными контроллерами.",
                    },
                    new TopicSeed
                    {
                        Name = "Драйвер t13",
                        Summary = "Поддержка обмена с устройствами через драйвер t13 сохранена в TRACE MODE для совместимости с предыдущими версиями, эту технологию не следует использовать для разработки новых драйверов.",
                    },
                    new TopicSeed
                    {
                        Name = "Драйверы t41/t12",
                        Summary = "Драйвер t41 по RS, с COM-портом работает МРВ",
                    },
                    new TopicSeed
                    {
                        Name = "Драйверы RWH обмена с УСО",
                        Summary = "Драйвер обмена с платами должен иметь имя rwh .",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-08-debug",
                Code = "08",
                Name = "Мониторы реального времени (МРВ)",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Монитор с поддержкой графических экранов",
                        Summary = "rtc. Дополнительные ключи команды запуска",
                    },
                    new TopicSeed
                    {
                        Name = "Задание параметров работы мониторов",
                        Summary = "Некоторые параметры работы мониторов могут быть заданы с помощью ключей команды запуска или с помощью конфигурационных файлов tmcom_[ordinal].cnf и tmcom7.cnf .",
                    },
                    new TopicSeed
                    {
                        Name = "Восстановление после рестарта",
                        Summary = "После рестарта значения атрибутов числовых каналов могут быть восстановлены из дампа (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Монитор EmbeddedRTM (rtme)",
                        Summary = "rtme. Замечания о конфигурировании",
                    },
                    new TopicSeed
                    {
                        Name = "Особенности работы в Windows и Linux",
                        Summary = "Различно местоположение файлов проекта.",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-09-lang",
                Code = "09",
                Name = "Программирование алгоритмов",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Встроенные алгоритмы (без написания программ)",
                        Summary = "Для задания алгоритмов функционирования разрабатываемого проекта АСУ в TRACE MODE могут быть использованы встроенные алгоритмы числовых каналов и каналов CALL (написание программ не требуется) – см.",
                    },
                    new TopicSeed
                    {
                        Name = "Программирование алгоритмов в TRACE MODE",
                        Summary = "Для программирования алгоритмов функционирования разрабатываемого проекта АСУ в TRACE MODE включены языки Техно ST и Техно FBD .",
                    },
                    new TopicSeed
                    {
                        Name = "Выполнение программы в реальном времени",
                        Summary = "Для выполнения программы в реальном времени в узле должен быть создан канал класса CALL с типом вызова Program , настроенный на вызов шаблона программы (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Редактор программ",
                        Summary = "Меню 'Программа' и панель инструментов редактора программ",
                    },
                    new TopicSeed
                    {
                        Name = "Окно структуры программы",
                        Summary = "В данном окне в виде дерева отображается структура программы.",
                    },
                    new TopicSeed
                    {
                        Name = "Выбор языка программирования",
                        Summary = "Язык программирования может быть независимо задан для основной программы и ее функций-блоков/функций.",
                    },
                    new TopicSeed
                    {
                        Name = "Панель инструментов рабочего поля редактора программ",
                        Summary = "Панель инструментов рабочего поля ST содержит только типовые инструменты:",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Свойства' редактора программ",
                        Summary = "Окно ’Свойства’ программы",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Протокол' редактора программ",
                        Summary = "Вкладка отображает ошибки, обнаруженные при компиляции.",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Отладчик' редактора программ",
                        Summary = "Вкладка аргументов окна отладчика",
                    },
                    new TopicSeed
                    {
                        Name = "Настройка редакторов и отладчика",
                        Summary = "Параметры редакторов и отладчика программ настраиваются в разделе Программы диалога Настройки ИС (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Лексическая структура языка Техно ST",
                        Summary = "прописные и строчные буквы латинского алфавита;",
                    },
                    new TopicSeed
                    {
                        Name = "Синтаксис Техно ST",
                        Summary = "Основная точка входа в программу",
                    },
                    new TopicSeed
                    {
                        Name = "Переменные и константы Техно ST",
                        Summary = "Под объектом в Техно ST понимается некоторая область памяти, которой присвоено имя (идентификатор).",
                    },
                    new TopicSeed
                    {
                        Name = "Определение переменных и констант",
                        Summary = "Особенности присвоения значений переменным",
                    },
                    new TopicSeed
                    {
                        Name = "Числовые константы Техно ST",
                        Summary = "Десятичные целочисленные константы состоят из ненулевой цифры, за которой следует последовательность десятичных цифр:",
                    },
                    new TopicSeed
                    {
                        Name = "Строковые константы Техно ST",
                        Summary = "Строковые константы представляют собой набор символов, заключенных в одинарные или двойные кавычки: ’первая строка’ , \" вторая строка \".",
                    },
                    new TopicSeed
                    {
                        Name = "Особенности вычислений",
                        Summary = "Целочисленность результата арифметических вычислений в программе имеет высший приоритет – даже в том случае, когда этот результат присваивается переменной с плавающей точкой.",
                    },
                    new TopicSeed
                    {
                        Name = "Символьные операторы",
                        Summary = "Арифметические операторы",
                    },
                    new TopicSeed
                    {
                        Name = "Операторы Техно ST",
                        Summary = "Операторы определения переменных",
                    },
                    new TopicSeed
                    {
                        Name = "Стандартные функции C в ST-программе",
                        Summary = "В ST-программе может быть использован ряд стандартных функций Си:",
                    },
                    new TopicSeed
                    {
                        Name = "Специальные функции в ST-программе",
                        Summary = "Для функций чтения и установки аргументов канала требуется, чтобы размерность массива buffer была больше или равна числу аргументов count .",
                    },
                    new TopicSeed
                    {
                        Name = "Пользовательские функции Техно ST",
                        Summary = "Определение функции и функции-блока",
                    },
                    new TopicSeed
                    {
                        Name = "Внешние библиотеки функций",
                        Summary = "Рекомендуется использовать возможность вызова функций из внешних библиотек ИСКЛЮЧИТЕЛЬНО для математических вычислений.",
                    },
                    new TopicSeed
                    {
                        Name = "Редактирование FBD-программ",
                        Summary = "Размещение FBD-блоков в рабочем поле редактора",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Логические'",
                        Summary = "Логическое сложение четырех элементов (||)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Побитовые'",
                        Summary = "Побитовое исключающее ИЛИ (X ^ Y)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Арифметические'",
                        Summary = "Сложение двух элементов (X+Y)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Тригонометрические'",
                        Summary = "Арктангенс отношения (_ATAN)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Алгебраические'",
                        Summary = "Натуральный логарифм (LN)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Функции сравнения'",
                        Summary = "Анализ на равенство (CMP)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Функции выбора'",
                        Summary = "Выбор максимального (MAX)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Триггеры и счетчики'",
                        Summary = "Импульс по переднему фронту (rTRIG)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Генераторы'",
                        Summary = "Случайная величина в диапазоне [0, 1] (RND)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Управление'",
                        Summary = "Экспоненциальное сглаживание (SMTH)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Переходы'",
                        Summary = "Выход из программы (EXIT)",
                    },
                    new TopicSeed
                    {
                        Name = "Раздел 'Регулирование'",
                        Summary = "Трехпозиционный регулятор (PREG)",
                    },
                    new TopicSeed
                    {
                        Name = "Создание пользовательских функциональных блоков",
                        Summary = "Для создания в TRACE MODE пользовательского функционального блока достаточно создать в программе функцию или функцию-блок на любом из встроенных языков.",
                    },
                    new TopicSeed
                    {
                        Name = "Отладка программ",
                        Summary = "Средства отладки включают в себя несколько режимов непрерывного и пошагового выполнения программы с возможностью установки точек останова.",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-10-graph",
                Code = "10",
                Name = "Разработка графического интерфейса",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Графический интерфейс, не требующий разработки",
                        Summary = "В TRACE MODE предусмотрены следующие средства графического представления данных и управления, не требующие разработки:",
                    },
                    new TopicSeed
                    {
                        Name = "Редактор представления данных",
                        Summary = "Типовые инструменты описаны в разделе Типовые средства редактирования .",
                    },
                    new TopicSeed
                    {
                        Name = "Задание параметров графического экрана",
                        Summary = "Чтобы открыть/закрыть окно параметров редактируемого графического экрана или графического объекта, нужно использовать команду Параметры экрана (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Особенности вызова графического экрана",
                        Summary = "Канал CALL.SCREEN с ОБРАБОТКА [36]=да",
                    },
                    new TopicSeed
                    {
                        Name = "Перепривязка аргументов экрана",
                        Summary = "Для перепривязки аргументов канала вызова экрана ( SCREEN ) может быть использована конфигурация, в которой в SCREEN задана привязка к атрибуту (124, ArgSize ) канала CALL.ChGroupReq ( root ), к ар...",
                    },
                    new TopicSeed
                    {
                        Name = "Открытие всплывающего экрана с перепривязкой аргументов",
                        Summary = "Конфигурирование механизма:",
                    },
                    new TopicSeed
                    {
                        Name = "Команда INFO SCREEN",
                        Summary = "Команда INFO ? SCREEN (может быть введена в любой атрибут канала в МРВ) выводит в протокол node . txt параметры экранов узла:",
                    },
                    new TopicSeed
                    {
                        Name = "Команда INFO GRAPH",
                        Summary = "Команда INFO ? GRAPH (может быть введена в любой атрибут канала в МРВ) выводит в протокол node . txt следующий список ключей:",
                    },
                    new TopicSeed
                    {
                        Name = "Операции с графическими слоями",
                        Summary = "Создание и удаление графических слоев",
                    },
                    new TopicSeed
                    {
                        Name = "Операции с графическими элементами",
                        Summary = "Перемещение и масштабирование ГЭ",
                    },
                    new TopicSeed
                    {
                        Name = "Задание типовых свойств ГЭ",
                        Summary = "В этом разделе описаны типовые свойства графических элементов и инструменты их задания.",
                    },
                    new TopicSeed
                    {
                        Name = "Статические атрибуты ГЭ",
                        Summary = "В данном разделе описано задание типовых статических атрибутов ГЭ с помощью вкладок окна Свойства .",
                    },
                    new TopicSeed
                    {
                        Name = "Динамизация атрибута ГЭ",
                        Summary = "Конфигурирование индикации значения",
                    },
                    new TopicSeed
                    {
                        Name = "Сочетания клавиш в окне 'Свойства'",
                        Summary = "ENTER – перейти к редактированию выделенного атрибута;",
                    },
                    new TopicSeed
                    {
                        Name = "Динамические свойства ГЭ",
                        Summary = "К динамическим свойствам графических элементов относятся динамическая заливка , 3 вида динамической трансформации ( перемещение , масштабирование и вращение ) и динамический контур .",
                    },
                    new TopicSeed
                    {
                        Name = "Динамическая заливка ГЭ",
                        Summary = "При использовании данного свойства ГЭ отображает значение привязанного аргумента числового формата в виде закрашенной области (такая область далее называется слоем ).",
                    },
                    new TopicSeed
                    {
                        Name = "Динамическая трансформация ГЭ",
                        Summary = "Динамическое перемещение ГЭ",
                    },
                    new TopicSeed
                    {
                        Name = "Динамический контур ГЭ",
                        Summary = "Динамический контур представляет собой прокручиваемый пунктир (под прокруткой здесь подразумевается дискретное перемещение с шагом, равным длине штриха).",
                    },
                    new TopicSeed
                    {
                        Name = "Функции управления ГЭ",
                        Summary = "Функция передачи значения",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Текст'",
                        Summary = "ГЭ 'Физическая величина'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Линии'",
                        Summary = "ГЭ Прямая не имеет специфических свойств и размещается в графическом слое стандартным способом (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Плоские фигуры'",
                        Summary = "ГЭ 'Скругленный прямоугольник'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Индикаторы'",
                        Summary = "ГЭ 'Знак в треугольнике'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Аппараты'",
                        Summary = "ГЭ данной группы размещаются в графическом слое стандартным способом (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Емкости'",
                        Summary = "ГЭ данной группы размещаются в графическом слое стандартным способом (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Агрегаты'",
                        Summary = "Общие специфические атрибуты ГЭ данной группы аналогичны атрибутам емкостей – см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Трубы и фитинги'",
                        Summary = "Общие специфические атрибуты ГЭ данной группы аналогичны атрибутам емкостей – см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Кнопки'",
                        Summary = "ГЭ данной группы размещаются в графическом слое стандартным способом (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Кнопки специальные'",
                        Summary = "Аналог ГЭ 'Группа кнопок' ( err _ model _ btn ), по умолчанию настроен только на управление по алгоритму arg = arg ^ Val ( Стиль кнопок = Кнопка , Кнопка XOR = TRUE ).",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Приборы'",
                        Summary = "ГЭ 'Битовый индикатор 8'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Тренды'",
                        Summary = "ГЭ Тренд размещается в графическом слое стандартным способом (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Вентиляция и кондиционирование'",
                        Summary = "Встроено две группы Вентиляция и кондиционирование .",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Элементы зданий'",
                        Summary = "ГЭ 'Прямоугольная комната'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Электрические элементы'",
                        Summary = "ГЭ 'Короткозамыкатель 2'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ '3D-фигуры Open GL'",
                        Summary = "3D-фигуры Open GL сохранены для совместимости с TRACE MODE 6.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Ресурсы из файла'",
                        Summary = "Данный ГЭ сохранен только для совместимости с TRACE MODE 6.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Растровые картинки'",
                        Summary = "При раскрытии в окне Графические элементы группы Растровые картинки открывается навигатор для выбора ресурса из библиотек изображений (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Векторные картинки'",
                        Summary = "При раскрытии в окне Графические элементы группы Векторные картинки открывается навигатор для выбора ресурса из библиотек SVG (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Видеоклипы'",
                        Summary = "При раскрытии в окне Графические элементы группы Видеоклипы открывается навигатор для выбора ресурса из библиотек видео (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Строки'",
                        Summary = "ГЭ 'Текст из библиотеки'",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Объекты'",
                        Summary = "При раскрытии в окне Графические элементы группы Объекты открывается навигатор для выбора ресурса из групп ГО (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Группа ГЭ 'Клипы'",
                        Summary = "Данная группа содержит ГЭ, анимация которых зависит от привязки.",
                    },
                    new TopicSeed
                    {
                        Name = "Операции с ресурсными библиотеками",
                        Summary = "Библиотеки рисунков и видеоклипов",
                    },
                    new TopicSeed
                    {
                        Name = "Встроенные окна мониторов",
                        Summary = "В мониторы встроены следующие окна, не требующие редактирования:",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-11-evrep",
                Code = "11",
                Name = "События",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "События",
                        Summary = "Отчет событий (EvRep, заменяет собой отчет тревог версии 6) – это в общем случае 2 бинарных файла, в которые могут записываться сообщения, которые генерирует МРВ в различных ситуациях при работе АСУ:",
                    },
                    new TopicSeed
                    {
                        Name = "Управление окнами событий",
                        Summary = "Посылка в атрибут 251 любого числового канала числа 8 приводит к открытию дополнительного окна EvRep, содержащего сообщения по этому каналу (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Открытие дополнительного окна событий при переходе на экран",
                        Summary = "В проекте может быть задано автоматическое открытие дополнительного окна событий при переходе на экран в МРВ (об открытии дополнительного окна вручную см.",
                    },
                    new TopicSeed
                    {
                        Name = "Словари сообщений",
                        Summary = "Общие замечания по сообщениям",
                    },
                    new TopicSeed
                    {
                        Name = "Категории сообщений",
                        Summary = "Сообщение может быть отнесено к одной из следующих категорий ( category ):",
                    },
                    new TopicSeed
                    {
                        Name = "Использование звуков",
                        Summary = "Cписок Звук содержит следующие опции ( destination ):",
                    },
                    new TopicSeed
                    {
                        Name = "SYS0",
                        Summary = "Системный словарь для пользователей",
                    },
                    new TopicSeed
                    {
                        Name = "SYS1",
                        Summary = "Системный словарь для узлов",
                    },
                    new TopicSeed
                    {
                        Name = "SYS2",
                        Summary = "Системный словарь для блокировок",
                    },
                    new TopicSeed
                    {
                        Name = "SYS3",
                        Summary = "Системный словарь для состояний каналов",
                    },
                    new TopicSeed
                    {
                        Name = "SYS4",
                        Summary = "Системный словарь для режимов управления",
                    },
                    new TopicSeed
                    {
                        Name = "SYS5",
                        Summary = "Системный словарь (вспомогательный)",
                    },
                    new TopicSeed
                    {
                        Name = "SYS6",
                        Summary = "Системный словарь для DPA VLV",
                    },
                    new TopicSeed
                    {
                        Name = "SYS7",
                        Summary = "Системный словарь для DPA APP и ECA",
                    },
                    new TopicSeed
                    {
                        Name = "SYS8",
                        Summary = "Системный словарь для DPA REG",
                    },
                    new TopicSeed
                    {
                        Name = "SYS9",
                        Summary = "Системный словарь для переходов границ (каналы ANALOG)",
                    },
                    new TopicSeed
                    {
                        Name = "SYS10",
                        Summary = "Системный словарь для режимов работы",
                    },
                    new TopicSeed
                    {
                        Name = "SYS11",
                        Summary = "Словарь для счетчиков (CNT-каналы)",
                    },
                    new TopicSeed
                    {
                        Name = "Файл is_attr_signaled.tmc",
                        Summary = "Файл tmcf \\ sys \\ is _ attr _ signaled .",
                    },
                    new TopicSeed
                    {
                        Name = "Окна событий в rtc",
                        Summary = "EvRep. Основные окна событий",
                    },
                    new TopicSeed
                    {
                        Name = "Ключи и команды для отчета событий",
                        Summary = "Команда INFO ? MESSAGE (может быть введена в любой атрибут канала в МРВ) выводит в протокол node . txt системные словари, а также пользовательские словари, созданные в проекте и используемые к...",
                    },
                    new TopicSeed
                    {
                        Name = "Отчет событий на внешних устройствах",
                        Summary = "В TRACE MODE поддерживаются следующие операции с EvRep на внешних устройствах:",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-12-arch",
                Code = "12",
                Name = "Архивирование",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Архивы SIAD",
                        Summary = "Архивы SIAD конфигурируются для узла (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Конфигурирование архивов SIAD",
                        Summary = "Архивы SIAD конфигурируются для узла на вкладке SIAD / Дамп .",
                    },
                    new TopicSeed
                    {
                        Name = "Выборка и обработка данных SIAD",
                        Summary = "Временной интервал выборки",
                    },
                    new TopicSeed
                    {
                        Name = "Обработка данных архива по каналам",
                        Summary = "Для статистической обработки данных архива по заданным каналам в диапазоне (T_FROM, T_TO) используются каналы CALL с типом вызова (29) LocalStatistic (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Выборка данных из архива по каналам",
                        Summary = "Для выборки данных архивов по заданным каналам из диапазона (T_FROM, T_TO) и вычисления по интервалам заданных характеристик используются каналы CALL с типом вызова (31) LocalList (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Повременные срезы архива",
                        Summary = "Для получения срезов архива используются каналы класса CALL с типом вызова (25) LocalSnap (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Архивы VCTR",
                        Summary = "Отображение архива VCTR на тренде",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-13-doc",
                Code = "13",
                Name = "Генерация документов",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Генерация документов",
                        Summary = "Использование разработанных шаблонов",
                    },
                    new TopicSeed
                    {
                        Name = "Редактор шаблонов документов (отчетов)",
                        Summary = "Задание параметров редактора документов",
                    },
                    new TopicSeed
                    {
                        Name = "Конфигурирование обычной таблицы",
                        Summary = "Таблица для вставки в документ выбирается в меню Вставить таблицу (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Конфигурирование таблицы архивных значений",
                        Summary = "В архивную таблицу поддерживается вывод данных из одного SIAD и вывод данных из разных VCTR.",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка значения переменной",
                        Summary = "В шаблоне документа могут быть использованы следующие выражения:",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка рисунка",
                        Summary = "Подменю Изображение меню Вставить объект содержит следующие команды:",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка выражения времени",
                        Summary = "Чтобы документ содержал значение времени генерации, в шаблон документа нужно вставить соответствующий объект по команде Дата и время из меню Вставить объект .",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка тренда",
                        Summary = "Для отображения архивных данных в виде тренда в шаблон документа нужно вставить соответствующий объект по команде Тренд из меню Вставить объект .",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка гистограммы",
                        Summary = "Для отображения в документе значений переменных в виде гистограммы в шаблон документа нужно вставить соответствующий объект по команде Гистограмма из меню Вставить объект .",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка круговой диаграммы",
                        Summary = "Для отображения в документе значений переменных в виде круговой диаграммы в шаблон документа нужно вставить соответствующий объект по команде Круговая диаграмма из меню Вставить объект .",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка отчета событий",
                        Summary = "Для отображения в документе данных отчета событий (EvRep, см.",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка расписания",
                        Summary = "Для отображения в документе расписания канала CALL.Scheduler (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Вставка документа",
                        Summary = "Возможность вставки документа в документ позволяет генерировать отчеты (далее – главные отчеты), содержащие данные нескольких отчетов, сгенерированных ранее, а также данные произвольных HTML-файлов.",
                    },
                    new TopicSeed
                    {
                        Name = "Документы на внешних устройствах",
                        Summary = "В TRACE MODE поддерживаются следующие операции с документами на внешних устройствах:",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-14-db",
                Code = "14",
                Name = "Обмен с базами данных",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Обмен с базами данных",
                        Summary = "Основными функциями, выполняемыми TRACE MODE при работе с базами данных (далее – БД), являются следующие:",
                    },
                    new TopicSeed
                    {
                        Name = "Синтаксис SQL",
                        Summary = "TRACE MODE не накладывает никаких ограничений на запросы к базам данных;",
                    },
                    new TopicSeed
                    {
                        Name = "Настройка параметров редактора связей с БД",
                        Summary = "Параметры редактора связей с БД настраиваются в разделе Базы данных (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Подключение к базе данных",
                        Summary = "При открытии редактор связей с БД имеет следующий вид:",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Схема'",
                        Summary = "Если подключение к БД выполнено корректно, в окне Схема редактора связей с БД отображается структура базы данных:",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Аргументы'",
                        Summary = "В окне Аргументы , представляющем собой редактор аргументов (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Протокол'",
                        Summary = "В данном окне отображается протокол работы редактора связей с БД (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Окно 'Запрос'",
                        Summary = "В данном окне редактируется SQL-запрос (см.",
                    },
                    new TopicSeed
                    {
                        Name = "Создание SQL-запросов",
                        Summary = "В шаблоне связи с БД может быть создан единственный SQL-запрос.",
                    },
                    new TopicSeed
                    {
                        Name = "Подстановки в SQL-запросе",
                        Summary = "Подстановка в SQL-запросе имеет следующий синтаксис:",
                    },
                    new TopicSeed
                    {
                        Name = "Создание SQL-запросов с помощью мастера",
                        Summary = "Построение запроса SELECT с помощью мастера",
                    },
                    new TopicSeed
                    {
                        Name = "Выполнение SQL-запросов из ИС",
                        Summary = "Окно Запрос снабжено следующими кнопками:",
                    },
                    new TopicSeed
                    {
                        Name = "Выполнение SQL-запросов в реальном времени",
                        Summary = "Формирование кривых в CALL.TVC",
                    },
                    new TopicSeed
                    {
                        Name = "Окно CALL.SQLQuery. Передача данных в CALL.Recipe",
                        Summary = "Данный функционал описан в подразделе Извлечение столбцов .",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-15-ems",
                Code = "15",
                Name = "EMS, система сообщений об ошибках",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "EMS, система сообщений об ошибках",
                        Summary = "# Система сообщений об ошибках TRACE MODE (EMS, tmems _ code )",
                    },
                }
            },
            new SectionSeed
            {
                Id = "tm7-sec-16-app",
                Code = "16",
                Name = "Приложения",
                Topics = new List<TopicSeed>
                {
                    new TopicSeed
                    {
                        Name = "Используемые сокращения",
                        Summary = "АСУ – автоматизированная система управления.",
                    },
                    new TopicSeed
                    {
                        Name = "Отличия версии 7 от версии 6",
                        Summary = "Вместо каналов FLOAT и DOUBLE FLOAT – единый канал ANALOG.",
                    },
                    new TopicSeed
                    {
                        Name = "Типовые средства редактирования",
                        Summary = "Типовые инструменты редактирования",
                    },
                    new TopicSeed
                    {
                        Name = "Задание позиций и размеров окон",
                        Summary = "В ИС позиция некоторых окон МРВ задается с помощью раздела следующего вида:",
                    },
                    new TopicSeed
                    {
                        Name = "Формат Си вывода чисел",
                        Summary = "Обозначению формата предшествует знак процента (%):",
                    },
                    new TopicSeed
                    {
                        Name = "Формат Си вывода даты и времени",
                        Summary = "Обозначению формата предшествует знак процента (%), реальный вывод зависит от региональных настроек ОС:",
                    },
                    new TopicSeed
                    {
                        Name = "Формат IP-адреса",
                        Summary = "В редакторах ИС IP-адрес задается в формате IPv4 или IPv6.",
                    },
                    new TopicSeed
                    {
                        Name = "События вывода дисплея из спящего состояния",
                        Summary = "Ниже приведен список событий, при возникновении которых дисплей выводится из спящего состояния (кроме событий нажатия клавиатуры или перемещения мыши):",
                    },
                }
            },
        };
    }

    private static List<ThemeSeed> BuildThemes()
    {
        return new List<ThemeSeed>
        {
            new ThemeSeed
            {
                Id = "tm7-theme-01-core",
                Code = "01",
                Title = "Тема 01: Архитектура и проектирование TRACE MODE 7",
                Description = "Экзаменационный блок по базовой архитектуре TRACE MODE 7, проектированию в ИС, системным переменным и разграничению доступа.",
                TimeMinutes = DefaultThemeTimeMinutes,
                SectionCodes = new List<string> { "00", "02", "04", "05" }
            },
            new ThemeSeed
            {
                Id = "tm7-theme-02-io",
                Code = "02",
                Title = "Тема 02: Каналы, источники и распределенные системы",
                Description = "Экзаменационный блок по каналам, источникам/приемникам, обмену, резервированию и распределенной архитектуре.",
                TimeMinutes = DefaultThemeTimeMinutes,
                SectionCodes = new List<string> { "03", "06", "07" }
            },
            new ThemeSeed
            {
                Id = "tm7-theme-03-runtime",
                Code = "03",
                Title = "Тема 03: МРВ, алгоритмы и графический интерфейс",
                Description = "Экзаменационный блок по выполнению проекта в МРВ, программированию алгоритмов, экранным формам и событиям.",
                TimeMinutes = DefaultThemeTimeMinutes,
                SectionCodes = new List<string> { "08", "09", "10", "11" }
            },
            new ThemeSeed
            {
                Id = "tm7-theme-04-integration",
                Code = "04",
                Title = "Тема 04: Архивы, документы, БД, EMS и приложения",
                Description = "Экзаменационный блок по архивированию, отчетности, интеграции с БД, системе сообщений и прикладным возможностям.",
                TimeMinutes = DefaultThemeTimeMinutes,
                SectionCodes = new List<string> { "12", "13", "14", "15", "16" }
            }
        };
    }

    private static Test BuildThemeTest(ThemeSeed theme, List<SectionSeed> allSections, int themeIndex)
    {
        var sectionByCode = (allSections ?? new List<SectionSeed>())
            .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Code))
            .ToDictionary(x => x.Code, StringComparer.Ordinal);

        var scopedSections = (theme.SectionCodes ?? new List<string>())
            .Where(code => sectionByCode.ContainsKey(code))
            .Select(code => sectionByCode[code])
            .ToList();

        var localTopics = scopedSections
            .SelectMany(section => (section.Topics ?? new List<TopicSeed>())
                .Where(IsUsableTopic)
                .Select(topic => new TopicContext { Section = section, Topic = topic }))
            .ToList();

        var allTopics = (allSections ?? new List<SectionSeed>())
            .SelectMany(section => (section.Topics ?? new List<TopicSeed>())
                .Where(IsUsableTopic)
                .Select(topic => new TopicContext { Section = section, Topic = topic }))
            .ToList();

        var questions = new List<Question>();
        for (var i = 0; i < localTopics.Count; i++)
        {
            var context = localTopics[i];
            var seedBase = themeIndex * 10000 + i * 37;

            questions.Add(BuildMeaningQuestion(context, localTopics, allTopics, seedBase + 11));
            questions.Add(BuildScenarioQuestion(context, localTopics, allTopics, seedBase + 29));
        }

        EnsureMinimumQuestionPool(questions, localTopics, allTopics, themeIndex * 701 + 13);
        questions = EnsureUniqueByText(questions);

        return new Test
        {
            Id = theme.Id,
            Title = theme.Title,
            Description = BuildThemeDescription(theme, scopedSections, questions.Count),
            TimeMinutes = Math.Max(20, theme.TimeMinutes),
            Questions = questions
        };
    }

    private static bool IsUsableTopic(TopicSeed topic)
    {
        if (topic == null || string.IsNullOrWhiteSpace(topic.Name))
            return false;

        // Не заставляем студента учить номера системных переменных и перечни атрибутов.
        var name = topic.Name.Trim();
        if (IsSystemVariableName(name))
            return false;

        if (IsAttributeCodeTopicName(name))
            return false;

        return true;
    }

    private static Question BuildMeaningQuestion(TopicContext context, List<TopicContext> localTopics, List<TopicContext> allTopics, int seed)
    {
        var examSummary = BuildExamSummary(context.Topic);
        return Single(
            BuildMeaningQuestionText(examSummary, seed),
            context.Topic.Name,
            BuildTopicNameOptions(context, localTopics, allTopics, seed)
        );
    }

    private static Question BuildScenarioQuestion(TopicContext context, List<TopicContext> localTopics, List<TopicContext> allTopics, int seed)
    {
        return Single(
            BuildScenarioQuestionText(BuildExamSummary(context.Topic), seed),
            context.Topic.Name,
            BuildTopicNameOptions(context, localTopics, allTopics, seed)
        );
    }

    private static void EnsureMinimumQuestionPool(List<Question> questions, List<TopicContext> localTopics, List<TopicContext> allTopics, int seed)
    {
        if (localTopics == null || localTopics.Count == 0)
            return;

        var guard = 0;
        while (EnsureUniqueByText(questions).Count < MinimumQuestionsPerTheme && guard < 5000)
        {
            var context = localTopics[guard % localTopics.Count];
            var questionSeed = seed + guard * 19;
            var summary = BuildExamSummary(context.Topic);

            questions.Add(Single(
                BuildControlQuestionText(summary, questionSeed),
                context.Topic.Name,
                BuildTopicNameOptions(context, localTopics, allTopics, questionSeed)
            ));

            guard++;
        }
    }

    private static string BuildThemeDescription(ThemeSeed theme, List<SectionSeed> scopedSections, int questionCount)
    {
        var sections = string.Join(
            ", ",
            (scopedSections ?? new List<SectionSeed>())
                .Select(s => string.Format("{0} {1}", s.Code, s.Name))
        );

        var description = (theme.Description ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            description = "Экзаменационный тест по теории TRACE MODE 7.";
        }

        description += "\n\nРазделы: " + sections;
        description += "\nВопросов в банке: " + questionCount;
        description += "\nВремя на попытку: 20 мин (1 вопрос = 1 мин).";
        description += "\nФормат: 20 случайных вопросов из пула.";
        return description;
    }

    private static string BuildMeaningQuestionText(string summary, int seed)
    {
        var templates = new[]
        {
            "В проекте нужно обеспечить: «{0}». Что в TRACE MODE 7 для этого используют?",
            "Какой инструмент TRACE MODE 7 применяют для задачи: «{0}»?",
            "При разработке АСУ требуется: «{0}». Что следует выбрать в TRACE MODE 7?",
            "Какое средство TRACE MODE 7 отвечает за «{0}»?",
            "Как в TRACE MODE 7 реализуют задачу: «{0}»?"
        };

        var template = templates[Math.Abs(seed) % templates.Length];
        return string.Format(template, summary);
    }

    private static string BuildScenarioQuestionText(string summary, int seed)
    {
        var templates = new[]
        {
            "Инженер настраивает систему. Требование: «{0}». Что нужно использовать в TRACE MODE 7?",
            "Для практической задачи «{0}» какой элемент TRACE MODE 7 выбирают?",
            "При запуске проекта нужно «{0}». Какой механизм TRACE MODE 7 подходит?",
            "Если в проекте требуется «{0}», какое решение выбирают в TRACE MODE 7?",
            "Какой компонент TRACE MODE 7 применяют для сценария: «{0}»?"
        };

        var template = templates[Math.Abs(seed) % templates.Length];
        return string.Format(template, summary);
    }

    private static string BuildControlQuestionText(string summary, int seed)
    {
        var templates = new[]
        {
            "Какой элемент TRACE MODE 7 нужен, чтобы выполнить: «{0}»?",
            "Что в TRACE MODE 7 используют для задачи: «{0}»?",
            "Какой модуль TRACE MODE 7 отвечает за «{0}»?",
            "Выберите рабочее решение TRACE MODE 7 для требования: «{0}»."
        };

        var template = templates[Math.Abs(seed) % templates.Length];
        return string.Format(template, summary);
    }

    private static string BuildExamSummary(TopicSeed topic)
    {
        var text = (topic?.Summary ?? string.Empty).Trim();
        text = Regex.Replace(text, @"https?://\S+", "официальный сайт TRACE MODE", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\(([^)]*см\.[^)]*)\)", string.Empty, RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\bсм\.[^.;:\n]*", string.Empty, RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\s+", " ").Trim();
        text = Regex.Replace(text, @"\(\s*$", string.Empty);
        text = text.Trim(' ', '-', ';', ':', '.', ',', '(', ')', '«', '»');

        if (string.IsNullOrWhiteSpace(text) || text.Length < 12)
        {
            text = "Назначение и практическое применение «" + (topic?.Name ?? "элемента") + "» в TRACE MODE 7.";
        }

        if (text.Length > 220)
        {
            var sentenceEnd = text.IndexOf('.');
            if (sentenceEnd > 40)
            {
                text = text.Substring(0, sentenceEnd + 1).Trim();
            }
        }

        if (text.Length > 220)
        {
            text = text.Substring(0, 217).TrimEnd() + "...";
        }

        return text;
    }

    private static string[] BuildTopicNameOptions(TopicContext correct, List<TopicContext> localTopics, List<TopicContext> allTopics, int seed)
    {
        var options = new List<string> { correct.Topic.Name };

        var local = (localTopics ?? new List<TopicContext>())
            .Where(t => !IsSameTopic(t, correct))
            .Select(t => t.Topic.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x) && !IsSystemVariableName(x) && !IsAttributeCodeTopicName(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        AddOptionsBySeed(options, local, seed * 13 + 5, 2);

        var global = (allTopics ?? new List<TopicContext>())
            .Where(t => !IsSameTopic(t, correct))
            .Select(t => t.Topic.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x) && !IsSystemVariableName(x) && !IsAttributeCodeTopicName(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        AddOptionsBySeed(options, global, seed * 31 + 9, 8);

        return FinalizeOptions(options, correct.Topic.Name, seed * 61 + 17);
    }

    private static bool IsSameTopic(TopicContext left, TopicContext right)
    {
        if (left == null || right == null || left.Topic == null || right.Topic == null)
            return false;

        return string.Equals(left.Topic.Name, right.Topic.Name, StringComparison.Ordinal) &&
               string.Equals(BuildExamSummary(left.Topic), BuildExamSummary(right.Topic), StringComparison.Ordinal);
    }

    private static void AddOptionsBySeed(List<string> target, List<string> source, int seed, int limit)
    {
        if (target == null || source == null || source.Count == 0 || limit <= 0)
            return;

        var start = Math.Abs(seed) % Math.Max(1, source.Count);
        var added = 0;

        for (var i = 0; i < source.Count && added < limit; i++)
        {
            var candidate = source[(start + i) % source.Count];
            if (string.IsNullOrWhiteSpace(candidate))
                continue;

            if (target.Any(x => string.Equals(x, candidate, StringComparison.Ordinal)))
                continue;

            target.Add(candidate);
            added++;
        }
    }

    private static string[] FinalizeOptions(IEnumerable<string> options, string correct, int seed)
    {
        var correctValue = (correct ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(correctValue))
            correctValue = "Корректный вариант";

        var all = (options ?? Enumerable.Empty<string>())
            .Select(x => (x ?? string.Empty).Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (!all.Any(x => string.Equals(x, correctValue, StringComparison.Ordinal)))
        {
            all.Insert(0, correctValue);
        }

        var distractors = all
            .Where(x => !string.Equals(x, correctValue, StringComparison.Ordinal))
            .ToList();

        var selectedDistractors = Shuffle(seed * 19 + 5, distractors)
            .Take(3)
            .ToList();

        while (selectedDistractors.Count < 3)
        {
            selectedDistractors.Add("Некорректный вариант " + (selectedDistractors.Count + 1));
        }

        var final = new List<string> { correctValue };
        final.AddRange(selectedDistractors);

        return Shuffle(seed * 113 + 7, final).Take(4).ToArray();
    }

    private static bool IsSystemVariableName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var text = value.Trim();
        if (text.Length < 2 || text.Length > 6)
            return false;

        if (text[0] != 's' && text[0] != 'S')
            return false;

        for (var i = 1; i < text.Length; i++)
        {
            if (!char.IsDigit(text[i]))
                return false;
        }

        return true;
    }

    private static bool IsAttributeCodeTopicName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var text = value.Trim();
        if (!text.StartsWith("Атрибут", StringComparison.OrdinalIgnoreCase))
            return false;

        // Примеры: "Атрибут 122", "Атрибуты 4, 46, 139-140"
        return Regex.IsMatch(text, @"\d");
    }

    private static Test BuildFinalMixed(List<Test> sourceTests)
    {
        var mixed = (sourceTests ?? new List<Test>())
            .SelectMany(t => t.Questions ?? new List<Question>())
            .Select(CloneQuestion)
            .ToList();

        return new Test
        {
            Id = "tm7-final-advanced",
            Title = "Итоговый экзамен: полный пул TRACE MODE 7",
            Description =
                "Сводный экзаменационный тест по всем темам TRACE MODE 7.\n" +
                "Включает весь пул вопросов из 4 тематических тестов.\n" +
                "Формат: 20 случайных вопросов из общего банка.\n" +
                "Время на попытку: 20 мин (1 вопрос = 1 мин).",
            TimeMinutes = 20,
            Questions = EnsureUniqueByText(mixed)
        };
    }

    private static List<Question> EnsureUniqueByText(IEnumerable<Question> questions)
    {
        var result = new List<Question>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var q in questions ?? Enumerable.Empty<Question>())
        {
            var text = (q.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            if (!seen.Add(text))
                continue;

            result.Add(q);
        }

        return result;
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

    private static Question Single(string text, string answer, params string[] options)
    {
        return new Question
        {
            Id = Guid.NewGuid().ToString(),
            Text = text,
            Type = QuestionType.Single,
            Options = options.Distinct(StringComparer.Ordinal).ToList(),
            Answer = answer
        };
    }

    private static string[] Shuffle(int seed, IEnumerable<string> options)
    {
        var rnd = new Random(seed * 97 + 13);
        return (options ?? Enumerable.Empty<string>())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(_ => rnd.Next())
            .ToArray();
    }

    private sealed class SectionSeed
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<TopicSeed> Topics { get; set; } = new List<TopicSeed>();
    }

    private sealed class TopicSeed
    {
        public string Name { get; set; }
        public string Summary { get; set; }
    }

    private sealed class ThemeSeed
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TimeMinutes { get; set; }
        public List<string> SectionCodes { get; set; } = new List<string>();
    }

    private sealed class TopicContext
    {
        public SectionSeed Section { get; set; }
        public TopicSeed Topic { get; set; }
    }
}
