namespace Core.Events;

public sealed class LibraryChangedEventArgs(
    string copyId,
    string readerId,
    LibraryEventKind kind,
    DateTime? dueDate) : EventArgs
{
    public string CopyId { get; } = copyId;
    public string ReaderId { get; } = readerId;
    public LibraryEventKind Kind { get; } = kind;
    public DateTime? DueDate { get; } = dueDate;
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}