using Core.Domain;

namespace Core.Tests;

public static class TestData
{
    public static List<BookCopy> Items() =>
    [
        BookCopy.Create("T-001", "978-0-13", "Clean Code"),
        BookCopy.Create("T-002", "978-0-13", "Clean Architecture"),
        BookCopy.Create("T-003", "978-0-201", "Design Patterns"),
        BookCopy.Create("T-004", "978-0-201", "Refactoring"),
        BookCopy.Create("T-005", "978-1-617", "C# in Depth")
    ];
}