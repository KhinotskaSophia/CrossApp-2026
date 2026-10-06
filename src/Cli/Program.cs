using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Copies());

var service = new LendingService(store);

Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");
if (useFile)
    Console.WriteLine($"Шлях до файлу: {dataPath}");

Console.WriteLine("\n--- 1. Сценарій: додавання та зміна стану примірника ---");
var newCopy = service.AddBook("978-0-13-110362-7", "The C Programming Language");
Console.WriteLine($"Додано: {newCopy}");

Console.WriteLine("Видаємо примірник читачеві...");
service.IssueCopy(newCopy.Id);
Console.WriteLine($"Поточний стан: {service.Find(newCopy.Id)}");

Console.WriteLine("\n--- 2. Всі примірники у фонді (перші 5) ---");
foreach (var copy in service.All().Take(5))
{
    Console.WriteLine($"  {copy.Id,-10} {copy.Isbn,-20} {copy.Title,-35} {(copy.IsIssued ? "[На руках]" : "[В наявності]")}");
}

Console.WriteLine("\n--- 3. Сценарій відмови (перевірка помилок) ---");
TryDo("Спроба видати неіснуючий примірник", () =>
{
    service.IssueCopy("NON-EXISTENT-ID");
});

TryDo("Спроба повторно видати вже виданий примірник", () =>
{
    service.IssueCopy(newCopy.Id);
});

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[ПОМИЛКА] {title}: дія пройшла без винятку!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ПЕРЕХОПЛЕНО]: {title} -> {ex.GetType().Name}: {ex.Message}");
    }
}