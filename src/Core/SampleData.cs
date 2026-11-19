using Core.Domain;

namespace Core;

public static class SampleData
{
    public static List<BookCopy> Copies() =>
    [
        BookCopy.Create("BC-001", "978-0-13", "Clean Code", genre: "Програмування"),
        BookCopy.Create("BC-002", "978-0-13", "Clean Architecture", genre: "Архітектура"),
        BookCopy.Create("BC-003", "978-0-13", "The Clean Coder", genre: "Практики"),
        BookCopy.Create("BC-004", "978-0-201", "Design Patterns", genre: "Архітектура"),
        BookCopy.Create("BC-005", "978-0-201", "Refactoring", genre: "Програмування"),
        BookCopy.Create("BC-006", "978-0-321", "Domain-Driven Design", genre: "Архітектура"),
        BookCopy.Create("BC-007", "978-0-596", "Head First Design Patterns", genre: "Архітектура"),
        BookCopy.Create("BC-008", "978-1-617", "C# in Depth", genre: "Програмування"),
        BookCopy.Create("BC-009", "978-1-617", "CLR via C#", genre: "Програмування"),
        BookCopy.Create("BC-010", "978-1-491", "Designing Data-Intensive Applications", genre: "Архітектура"),
        BookCopy.Create("BC-011", "978-0-7356", "Code Complete", genre: "Практики"),
        BookCopy.Create("BC-012", "978-0-201", "Extreme Programming", genre: "Практики"),
        BookCopy.Create("BC-013", "978-0-13", "Working Effectively with Legacy Code", genre: "Практики"),
        BookCopy.Create("BC-014", "978-1-617", "Pro C# with .NET", genre: "Програмування"),
        BookCopy.Create("BC-015", "978-0-321", "Effective C#", genre: "Програмування")
    ];

    public static List<Loan> InitialLoans(List<BookCopy> copies, int loanDays = 14) =>
    [
        Loan.Open("L-001", copies[0], "Reader-Alpha", DateTime.Now.AddDays(-5)),
        Loan.Open("L-002", copies[3], "Reader-Beta", DateTime.Now.AddDays(loanDays - 9)),
        Loan.Open("L-003", copies[5], "Reader-Alpha", DateTime.Now.AddDays(-(loanDays + 6))),
        Loan.Open("L-004", copies[7], "Reader-Gamma", DateTime.Now.AddDays(-2))
    ];
}