using Core.Domain;

Console.WriteLine("=== Сценарій 1: успіх ===");

var copy = BookCopy.Create("BC-001", "978-0-13-235088-4", "Clean Code");
Console.WriteLine($"Створено: {copy}");

var loan = Loan.Open("LN-101", copy, "R-007", new DateTime(2026, 10, 1));
Console.WriteLine($"Оформлено: {loan}");
Console.WriteLine($"Стан примірника: {copy}");

loan.Close(copy, new DateTime(2026, 10, 5));
Console.WriteLine($"Після повернення: {loan}");
Console.WriteLine($"Стан примірника: {copy}");

Console.WriteLine();
Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

TryDo("Порожній ISBN примірника", () =>
{
    BookCopy.Create("BC-002", "   ", "Refactoring");
});

var busyCopy = BookCopy.Create("BC-003", "978-0-201-48567-7", "DDD");
var activeLoan = Loan.Open("LN-102", busyCopy, "R-001", new DateTime(2026, 10, 2));

TryDo("Повторна видача вже виданого примірника", () =>
{
    Loan.Open("LN-103", busyCopy, "R-002", new DateTime(2026, 10, 3));
});

TryDo("Дата повернення раніша за дату видачі", () =>
{
    activeLoan.Close(busyCopy, new DateTime(2026, 9, 30));
});

TryDo("Повторне закриття вже закритої видачі", () =>
{
    loan.Close(copy, new DateTime(2026, 10, 6));
});

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[ПОМИЛКА ТЕСТУ] {title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ОК] {title}: {ex.GetType().Name} -> {ex.Message}");
    }
}