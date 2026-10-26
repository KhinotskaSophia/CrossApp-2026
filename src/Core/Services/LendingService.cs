using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<LibraryChangedEventArgs>? Changed;

    private void OnChanged(string copyId, string readerId, LibraryEventKind kind, DateTime? dueDate = null) =>
        Changed?.Invoke(this, new LibraryChangedEventArgs(copyId, readerId, kind, dueDate));

    public BookCopy AddBook(string isbn, string title)
    {
        string id = $"BC-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
        var copy = BookCopy.Create(id, isbn, title);
        _store.Add(copy);

        OnChanged(copy.Id, "—", LibraryEventKind.BookAdded);
        return copy;
    }

    public void IssueCopy(string copyId, string readerId, DateTime dueDate)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id={copyId} не знайдено.");

        copy.Issue(); 
        _store.Update(copy);
        OnChanged(copy.Id, readerId, LibraryEventKind.CopyIssued, dueDate);
    }

    public void ReturnCopy(string copyId)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id={copyId} не знайдено.");

        copy.Return(); 
        _store.Update(copy);
        OnChanged(copy.Id, "—", LibraryEventKind.CopyReturned);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();

    public BookCopy? Find(string id) => _store.GetById(id);

    public IReadOnlyList<BookCopy> FindAll(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}