using Core.Domain;
using Core.Reports;
using Xunit;

namespace Core.Tests;

public class ReportServiceTests
{
    [Theory]
    [InlineData("Програмування", 2)]
    [InlineData("Класика", 2)]
    [InlineData("Неіснуючий жанр", 0)]
    public void GroupByGenre_FiltersAndGroupsCorrectly(string genreKey, int expectedCount)
    {
        var items = new List<BookCopy>
        {
            BookCopy.Create("T-001", "978-0-13", "Clean Code", genre: "Програмування"),
            BookCopy.Create("T-002", "978-0-13", "Clean Architecture", genre: "Програмування"),
            BookCopy.Create("T-003", "978-0-201", "Design Patterns", genre: "Класика"),
            BookCopy.Create("T-004", "978-0-201", "Refactoring", genre: "Класика"),
            BookCopy.Create("T-005", "978-1-617", "C# in Depth", genre: "Інше")
        };

        var reports = LibraryReportService.GroupByGenre(items);
        var targetGroup = reports.FirstOrDefault(g => g.Genre == genreKey);

        int actualCount = targetGroup?.BooksCount ?? 0;
        Assert.Equal(expectedCount, actualCount);
        Assert.Equal(reports.Select(r => r.Genre).ToList(),
                     reports.Select(r => r.Genre).OrderBy(g => g, StringComparer.CurrentCulture).ToList());
    }

    [Fact]
    public void TopReaders_IsDeterministic()
    {
        var copy = TestData.Items()[0];
        var loan1 = Loan.Open("L-1", copy, "Reader-A", DateTime.Now);

        var copy2 = TestData.Items()[1];
        var loan2 = Loan.Open("L-2", copy2, "Reader-B", DateTime.Now);

        var loans = new List<Loan> { loan1, loan2 };
        var top = LibraryReportService.TopReaders(loans, 2);

        Assert.Equal(2, top.Count);
        Assert.Equal(new[] { "Reader-A", "Reader-B" }, top.Select(r => r.ReaderId).OrderBy(x => x));
    }
}