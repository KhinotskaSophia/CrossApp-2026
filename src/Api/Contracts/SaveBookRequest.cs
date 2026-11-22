namespace Api.Contracts;

public sealed record SaveBookRequest(
    string Isbn,
    string Title,
    string Author,
    int Year,
    int Copies,
    string Genre)
{
    public bool TryValidate(out Dictionary<string, string[]> errors)
    {
        var found = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Isbn) || Isbn.Trim().Length > 32)
            found[nameof(Isbn)] = ["ISBN обов'язковий (до 32 символів)."];

        if (string.IsNullOrWhiteSpace(Title) || Title.Trim().Length > 200)
            found[nameof(Title)] = ["Назва обов'язкова (до 200 символів)."];

        if (string.IsNullOrWhiteSpace(Author) || Author.Trim().Length > 150)
            found[nameof(Author)] = ["Автор обов'язковий (до 150 символів)."];

        if (string.IsNullOrWhiteSpace(Genre) || Genre.Trim().Length > 100)
            found[nameof(Genre)] = ["Жанр обов'язковий (до 100 символів)."];

        var maxYear = DateTime.UtcNow.Year + 1;
        if (Year < 0 || Year > maxYear)
            found[nameof(Year)] = [$"Рік видання має бути в межах від 0 до {maxYear}."];

        if (Copies <= 0)
            found[nameof(Copies)] = ["Кількість примірників має бути більшою за 0."];

        errors = found;
        return found.Count == 0;
    }
}