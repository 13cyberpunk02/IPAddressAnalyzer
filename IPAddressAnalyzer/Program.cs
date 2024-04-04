using IPAddressAnalyzer;

var ipParser = new ParserExtensions();

// Парсинг аргументов командной строки
var arguments = ipParser.ParseArguments(args);
if(arguments is null)
{
    Console.WriteLine("Неверный формат аргументов командной строки.");
    return;
}

// Проверка наличия всех необходимых параметров
if (!arguments.ContainsKey("--file-log") || !arguments.ContainsKey("--file-output"))
{
    Console.WriteLine("Необходимые параметры не были указаны.");
    return;
}

// Чтение логов из файла
var logFilePath = arguments["--file-log"];
var logs = ipParser.ReadLogs(logFilePath);
if (logs is null)
{
    Console.WriteLine("Не удалось прочитать файл с логами.");
    return;
}

// Фильтрация логов по заданным параметрам
var filteredLogs = ipParser.FilterLogs(logs, arguments);
if (filteredLogs == null)
{
    Console.WriteLine("Ошибка при фильтрации логов.");
    return;
}

// Запись результатов в файл
var outputFile = arguments["--file-output"];
ipParser.WriteResults(outputFile, filteredLogs);