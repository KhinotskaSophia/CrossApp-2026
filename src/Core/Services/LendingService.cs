using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public BookCopy AddBook(string isbn, string title)
    {
        string id = $"BC-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var copy = BookCopy.Create(id, isbn, title);
        _store.Add(copy);
        return copy;
    }

    public void IssueCopy(string copyId)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id={copyId} не знайдено.");

        copy.Issue(); // доменний інваріант тижня 4
        _store.Update(copy);
    }

    public void ReturnCopy(string copyId)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id={copyId} не знайдено.");

        copy.Return(); // доменний інваріант тижня 4
        _store.Update(copy);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();

    public BookCopy? Find(string id) => _store.GetById(id);
}