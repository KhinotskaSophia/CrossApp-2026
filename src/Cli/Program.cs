using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Copies());

var service = new LendingService(store);
var allCopies = service.All();
var allLoans = SampleData.InitialLoans(allCopies.ToList());

var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-totals";

switch (command)
{
    case "report-top":
        var top = LibraryReportService.TopReaders(allLoans, 5);
        Console.WriteLine("=== ТОП ЧИТАЧІВ ЗА ВИДАЧАМИ ===");
        foreach (var r in top)
            Console.WriteLine($" Читач: {r.ReaderId,-12} | Активних видач: {r.LoansCount,3}");
        break;

    case "report-overdue":
        var overdue = LibraryReportService.OverdueLoans(allLoans, DateTime.Now);
        Console.WriteLine("=== ПРОСТРОЧЕНІ ВИДАЧІ ===");
        foreach (var o in overdue)
            Console.WriteLine($" ID: {o.LoanId,-6} | Примірник: {o.CopyId,-8} | Читач: {o.ReaderId,-12} | Дедлайн: {o.DueDate:yyyy-MM-dd}");
        break;

    case "report-groups":
        var groups = LibraryReportService.GroupByGenre(allCopies);
        Console.WriteLine("=== ГРУПУВАННЯ ЗА КАТЕГОРІЄЮ ISBN ===");
        foreach (var g in groups)
            Console.WriteLine($" {g.Genre,-6}  позицій: {g.BooksCount,3}  сумарно видано: {g.ActiveLoansCount,3}");
        break;

    case "report-totals":
        var t = LibraryReportService.Totals(allCopies, allLoans);
        Console.WriteLine($" позицій {t.TotalCopies,2}, всього примірників на руках {t.IssuedCopies}, середнє на категорію, макс копій {t.MaxCopiesPerBook}");
        break;

    case "demo-deferred":
        RunDeferredExecutionDemo(allCopies);
        break;

    default:
        Console.WriteLine("Доступні підкоманди звітів:");
        Console.WriteLine("  report-top | report-overdue | report-groups | report-totals | demo-deferred");
        break;
}

static void RunDeferredExecutionDemo(IReadOnlyList<BookCopy> sourceList)
{
    Console.WriteLine("=== ДЕМОНСТРАЦІЯ ВІДКЛАДЕНОГО ВИКОНАННЯ (DEFERRED EXECUTION) ===");
    
    var workingSource = sourceList.ToList();
    
    var query = workingSource.Where(c => c.Isbn.StartsWith("978-0-13"));
    
    Console.WriteLine($"Кількість до змін у джерелі: {query.Count()}");
    
    workingSource.Add(BookCopy.Create("BC-999", "978-0-13-999", "Dynamic Test Book"));
    
    Console.WriteLine($"Кількість ПІСЛЯ додавання нового елемента: {query.Count()}");

    var snapshot = query.ToList();
    workingSource.Clear();
    
    Console.WriteLine($"Після очищення джерела, у матеріалізованому знімку лишилось: {snapshot.Count} (дані зафіксовані).");
}