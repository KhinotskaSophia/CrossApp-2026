using Core.Domain;
using Core.Reports;
using Xunit;

namespace Core.Tests;

public class ReportServiceTests
{
    [Theory]
    [InlineData("978-0-13", 2)]
    [InlineData("978-0-201", 2)]
    [InlineData("NON-EXISTENT", 0)]
    public void GroupByGenre_FiltersAndGroupsCorrectly(string genreKey, int expectedCount)
    {
        var reports = LibraryReportService.GroupByGenre(TestData.Items());
        var targetGroup = reports.FirstOrDefault(g => g.Genre == genreKey);

        int actualCount = targetGroup?.BooksCount ?? 0;
        Assert.Equal(expectedCount, actualCount);
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