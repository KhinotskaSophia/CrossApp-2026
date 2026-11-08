namespace Core.Reports;

public sealed record TopReaderSummary(string ReaderId, int LoansCount);

public sealed record OverdueLoanLine(string LoanId, string CopyId, string ReaderId, DateTime DueDate);

public sealed record GenreGroupSummary(string Genre, int BooksCount, int ActiveLoansCount);

public sealed record LibraryTotalsSummary(int TotalCopies, int IssuedCopies, double PercentageIssued, int MaxCopiesPerBook);