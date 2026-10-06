using Core.Domain;

namespace Core;

public static class SampleData
{
    public static List<BookCopy> Copies() =>
    [
        BookCopy.Create("BC-001", "978-0-13-235088-4", "Clean Code"),
        BookCopy.Create("BC-002", "978-0-13-235088-4", "Clean Code"),
        BookCopy.Create("BC-003", "978-0-13-597444-5", "The Pragmatic Programmer"),
        BookCopy.Create("BC-004", "978-0-201-63361-0", "Design Patterns"),
        BookCopy.Create("BC-005", "978-0-201-63361-0", "Design Patterns"),
        BookCopy.Create("BC-006", "978-0-321-12521-7", "Domain-Driven Design"),
        BookCopy.Create("BC-007", "978-0-13-449416-6", "Clean Architecture"),
        BookCopy.Create("BC-008", "978-0-596-00712-6", "Head First Design Patterns"),
        BookCopy.Create("BC-009", "978-1-61729-454-9", "C# in Depth"),
        BookCopy.Create("BC-010", "978-1-61729-454-9", "C# in Depth"),
        BookCopy.Create("BC-011", "978-1-491-98765-0", "Designing Data-Intensive Applications"),
        BookCopy.Create("BC-012", "978-0-13-708107-3", "The Clean Coder"),
        BookCopy.Create("BC-013", "978-0-321-20068-6", "Refactoring"),
        BookCopy.Create("BC-014", "978-0-201-48567-7", "Extreme Programming Explained"),
        BookCopy.Create("BC-015", "978-0-7356-1967-8", "Code Complete")
    ];
}