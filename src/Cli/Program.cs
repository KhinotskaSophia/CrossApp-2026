using Core;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Copies());

var service = new LendingService(store);

var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnLibraryChanged(object? sender, LibraryChangedEventArgs e)
{
    string details = e.Kind switch
    {
        LibraryEventKind.BookAdded => "Додано нову книгу до фонду",
        LibraryEventKind.CopyIssued => $"Видано читачу {e.ReaderId} до {e.DueDate:yyyy-MM-dd}",
        LibraryEventKind.CopyReturned => "Примірник повернуто до бібліотеки",
        _ => string.Empty
    };

    string line = $"{e.At:HH:mm:ss} [{e.Kind}] Примірник ID: {e.CopyId} | {details}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnLibraryChanged;

int changesCount = 0;
service.Changed += (_, _) => changesCount++;

Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");

Console.WriteLine("\n--- Успішні операції ---");
var newCopy = service.AddBook("978-0-13-235088-4", "Clean Architecture");
service.IssueCopy(newCopy.Id, "R-007", DateTime.Now.AddDays(14));

Console.WriteLine($"\n[Лічильник] Подій отримано обробником-лямбдою: {changesCount}");

Console.WriteLine("\n--- Тестування відписки ---");
service.Changed -= OnLibraryChanged;

service.ReturnCopy(newCopy.Id);
Console.WriteLine($"[Лічильник після відписки]: {changesCount} (збільшився лише на 1 від лямбди)");

Console.WriteLine("\n--- Сценарій відмови (порушення інваріантів) ---");
TryDo("Спроба видати примірник, якого не існує", () =>
{
    service.IssueCopy("NON-EXISTENT", "R-001", DateTime.Now);
});

Console.WriteLine($"[Лічильник після помилки]: {changesCount} (помилка не згенерувала подію)");

Console.WriteLine("\n--- Використання Func (FindAll) замість події ---");

var issuedCopies = service.FindAll(b => b.IsIssued);
Console.WriteLine($"Знайдено примірників на руках через FindAll: {issuedCopies.Count}");

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[ПОМИЛКА ТЕСТУ] {title}: виняток не спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ОК] {title} -> {ex.GetType().Name}: {ex.Message}");
    }
}