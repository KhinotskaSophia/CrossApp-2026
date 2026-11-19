using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    public string Id { get; }
    public string Isbn { get; }
    public string Title { get; }
    public string Author { get; }
    public int Year { get; }
    public int Copies { get; }
    public string Genre { get; }
    public bool IsIssued { get; private set; }

    private BookCopy(
        string id,
        string isbn,
        string title,
        string author,
        int year,
        int copies,
        string genre,
        bool isIssued)
    {
        Id = id;
        Isbn = isbn;
        Title = title;
        Author = author;
        Year = year;
        Copies = copies;
        Genre = genre;
        IsIssued = isIssued;
    }

    public static BookCopy Create(
        string id,
        string isbn,
        string title,
        bool isIssued = false,
        string author = "Невідомий автор",
        int year = 0,
        int copies = 1,
        string genre = "Загальне")
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор примірника обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN не може бути порожнім", nameof(isbn));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва книги не може бути порожньою", nameof(title));

        if (year < 0)
            throw new ArgumentOutOfRangeException(nameof(year), "Рік видання не може бути від'ємним");

        if (copies <= 0)
            throw new ArgumentOutOfRangeException(nameof(copies), "Кількість примірників має бути більшою за 0");

        var normalizedAuthor = string.IsNullOrWhiteSpace(author) ? "Невідомий автор" : author.Trim();
        var normalizedGenre = string.IsNullOrWhiteSpace(genre) ? "Загальне" : genre.Trim();

        return new BookCopy(
            id.Trim(),
            isbn.Trim().ToUpperInvariant(),
            title.Trim(),
            normalizedAuthor,
            year,
            copies,
            normalizedGenre,
            isIssued);
    }

    public void Issue()
    {
        if (IsIssued)
            throw new InvalidOperationException($"Примірник {Id} вже виданий, повторна видача неможлива");

        IsIssued = true;
    }

    public void Return()
    {
        if (!IsIssued)
            throw new InvalidOperationException($"Примірник {Id} не був виданий, повернення неможливе");

        IsIssued = false;
    }

    public BookCopyDto ToDto() => new(Id, Isbn, Title, IsIssued, Author, Year, Copies, Genre);

    public static BookCopy FromDto(BookCopyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return Create(
            dto.Id,
            dto.Isbn,
            dto.Title,
            dto.IsIssued,
            dto.Author ?? "Невідомий автор",
            dto.Year,
            dto.Copies <= 0 ? 1 : dto.Copies,
            dto.Genre ?? "Загальне");
    }

    public override string ToString() =>
        $"{Id} [{Isbn}] ({Genre}) \"{Title}\" — {(IsIssued ? "На руках" : "В наявності")}";
}