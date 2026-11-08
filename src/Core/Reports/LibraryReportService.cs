using Core.Domain;

namespace Core.Reports;

public static class LibraryReportService
{
    public static IReadOnlyList<TopReaderSummary> TopReaders(IEnumerable<Loan> loans, int n) =>
        loans.Where(l => !l.IsClosed)
             .GroupBy(l => l.ReaderId, StringComparer.OrdinalIgnoreCase)
             .Select(g => new TopReaderSummary(g.Key, g.Count()))
             .OrderByDescending(r => r.LoansCount)
             .ThenBy(r => r.ReaderId, StringComparer.Ordinal)
             .Take(n)
             .ToList();

    public static IReadOnlyList<OverdueLoanLine> OverdueLoans(IEnumerable<Loan> loans, DateTime currentDate) =>
        loans.Where(l => !l.IsClosed && l.IssuedOn.AddDays(14) < currentDate)
             .OrderBy(l => l.IssuedOn)
             .ThenBy(l => l.Id, StringComparer.Ordinal)
             .Select(l => new OverdueLoanLine(l.Id, l.BookCopyId, l.ReaderId, l.IssuedOn.AddDays(14)))
             .ToList();

    public static IReadOnlyList<GenreGroupSummary> GroupByGenre(IEnumerable<BookCopy> copies) =>
        copies.GroupBy(c => c.Isbn.Length >= 6 ? c.Isbn[..6] : c.Isbn, StringComparer.OrdinalIgnoreCase)
              .Select(g => new GenreGroupSummary(g.Key, g.Count(), g.Count(c => c.IsIssued)))
              .OrderByDescending(g => g.BooksCount)
              .ThenBy(g => g.Genre, StringComparer.Ordinal)
              .ToList();

    public static LibraryTotalsSummary Totals(IEnumerable<BookCopy> copies, IEnumerable<Loan> loans)
    {
        var copyList = copies.ToList();
        var loanList = loans.Where(l => !l.IsClosed).ToList();

        if (copyList.Count == 0)
            return new LibraryTotalsSummary(0, 0, 0d, 0);

        int total = copyList.Count;
        int issued = loanList.Count;
        double percentage = (double)issued / total * 100;
        
        int maxPerBook = copyList.GroupBy(c => c.Isbn)
                                 .Max(g => (int?)g.Count()) ?? 0;

        return new LibraryTotalsSummary(total, issued, percentage, maxPerBook);
    }
}