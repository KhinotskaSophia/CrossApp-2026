using System.Reflection;
using Core;
using Core.Abstractions;
using Core.Configuration;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;
using Microsoft.Extensions.Configuration;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nОтримано сигнал скасування (Ctrl+C)...");
};

var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: false)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .AddCommandLine(args)
    .Build();

var options = config.Get<AppOptions>() ?? new AppOptions();
var legacyDataPath = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");

var effectiveStorage = new StorageOptions
{
    DataPath = string.IsNullOrWhiteSpace(legacyDataPath) ? options.Storage.DataPath : legacyDataPath,
    FileName = options.Storage.FileName
};

string fullDataPath = effectiveStorage.ResolveFullPath(AppContext.BaseDirectory);
var apiKey = config["Integration:ApiKey"];

Console.WriteLine($"Середовище: {envName} (Log: {options.Logging.MinimumLevel})");
Console.WriteLine($"Файл даних: {fullDataPath}");
Console.WriteLine(apiKey is null
    ? "ApiKey: не заданий"
    : $"ApiKey: присутній, довжина {apiKey.Length}");

bool useFile = args.Contains("--file");

IBookStore store = useFile
    ? new FileCatalogStore(fullDataPath)
    : new InMemoryBookStore(SampleData.Copies());

var service = new LendingService(store);

var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-totals";

try
{
    switch (command)
    {
        case "import":
            string importPath = fullDataPath;
            var pathIndex = Array.IndexOf(args, "--path");
            if (pathIndex >= 0 && pathIndex < args.Length - 1)
            {
                var rawPath = args[pathIndex + 1];
                importPath = Path.IsPathRooted(rawPath)
                    ? Path.GetFullPath(rawPath)
                    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, rawPath));
            }

            Console.WriteLine($"Розпочинаємо асинхронний імпорт із файлу: {importPath}");
            int imported = await service.ImportAsync(new JsonCatalogImporter(), importPath, cts.Token);
            Console.WriteLine($"Успішно імпортовано записів: {imported}");
            break;

        case "export":
            var fileStore = new FileCatalogStore(fullDataPath);
            foreach (var copy in service.All())
            {
                if (fileStore.GetById(copy.Id) is null)
                {
                    fileStore.Add(copy);
                }
            }
            fileStore.Export();
            Console.WriteLine($"Експортовано записів у {fullDataPath}: {fileStore.List().Count}");
            break;

        case "report-top":
            var allCopies = service.All().ToList();
            var allLoans = SampleData.InitialLoans(allCopies);
            var top = LibraryReportService.TopReaders(allLoans, 5);
            Console.WriteLine("=== ТОП ЧИТАЧІВ ЗА ВИДАЧАМИ ===");
            foreach (var r in top)
                Console.WriteLine($" Читач: {r.ReaderId,-12} | Активних видач: {r.LoansCount,3}");
            break;

        case "report-overdue":
            var copiesForOverdue = service.All().ToList();
            var loansForOverdue = SampleData.InitialLoans(copiesForOverdue);
            var overdue = LibraryReportService.OverdueLoans(loansForOverdue, DateTime.Now);
            Console.WriteLine("=== ПРОСТРОЧЕНІ ВИДАЧІ ===");
            foreach (var o in overdue)
                Console.WriteLine($" ID: {o.LoanId,-6} | Примірник: {o.CopyId,-8} | Читач: {o.ReaderId,-12} | Дедлайн: {o.DueDate:yyyy-MM-dd}");
            break;

        case "report-groups":
            var groups = LibraryReportService.GroupByGenre(service.All());
            Console.WriteLine("=== ГРУПУВАННЯ ЗА ЖАНРОМ ===");
            foreach (var g in groups)
                Console.WriteLine($" Жанр: {g.Genre,-20} | Позицій: {g.BooksCount,3}");
            break;

        case "report-totals":
            var copiesForTotals = service.All().ToList();
            var loansForTotals = SampleData.InitialLoans(copiesForTotals);
            var t = LibraryReportService.Totals(copiesForTotals, loansForTotals);
            Console.WriteLine($"Всього позицій: {t.TotalCopies}, примірників на руках: {t.IssuedCopies}");
            break;

        case "demo-deferred":
            RunDeferredExecutionDemo(service.All().ToList());
            break;

        default:
            Console.WriteLine($"Невідома команда: {command}");
            Console.WriteLine("Доступні підкоманди: import, export, report-top, report-overdue, report-groups, report-totals, demo-deferred");
            break;
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Імпорт скасовано.");
}
catch (DomainException ex)
{
    Console.WriteLine($"Дані відхилено: {ex.Message}");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"Файл не знайдено: {ex.FileName}");
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка виконання: {ex.Message}");
}

static void RunDeferredExecutionDemo(IReadOnlyList<BookCopy> sourceList)
{
    Console.WriteLine("=== ДЕМОНСТРАЦІЯ ВІДКЛАДЕНОГО ВИКОНАННЯ (DEFERRED EXECUTION) ===");
    var workingSource = sourceList.ToList();
    var query = workingSource.Where(c => c.Isbn.StartsWith("978-0-13"));
    Console.WriteLine($"Кількість до змін у джерелі: {query.Count()}");
    workingSource.Add(BookCopy.Create("BC-999", "978-0-13-999", "Dynamic Test Book"));
    Console.WriteLine($"Кількість ПІСЛЯ додавання нового елемента: {query.Count()}");
}