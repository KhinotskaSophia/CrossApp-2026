using Core.Dto;

namespace Core.Domain;

public sealed class Loan
{
    public string Id { get; }
    public string BookCopyId { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    public DateTime? ReturnedOn { get; private set; }

    public bool IsClosed => ReturnedOn.HasValue;

    private Loan(string id, string bookCopyId, string readerId, DateTime issuedOn, DateTime? returnedOn)
    {
        Id = id;
        BookCopyId = bookCopyId;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        ReturnedOn = returnedOn;
    }

    public static Loan Open(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        ArgumentNullException.ThrowIfNull(copy);

        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор видачі обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(readerId))
            throw new ArgumentException("Ідентифікатор читача обов'язковий", nameof(readerId));

        // Зміна стану примірника захищена його внутрішнім інваріантом
        copy.Issue();

        return new Loan(id.Trim(), copy.Id, readerId.Trim(), issuedOn, null);
    }

    public void Close(BookCopy copy, DateTime returnedOn)
    {
        ArgumentNullException.ThrowIfNull(copy);

        if (copy.Id != BookCopyId)
            throw new InvalidOperationException($"Примірник {copy.Id} не відповідає оформленій видачі {BookCopyId}");

        if (IsClosed)
            throw new InvalidOperationException($"Видача {Id} вже була закрита {ReturnedOn:yyyy-MM-dd}");

        if (returnedOn < IssuedOn)
            throw new ArgumentOutOfRangeException(
                nameof(returnedOn),
                returnedOn,
                $"Дата повернення ({returnedOn:yyyy-MM-dd}) не може передувати даті видачі ({IssuedOn:yyyy-MM-dd})");

        copy.Return();
        ReturnedOn = returnedOn;
    }

    public LoanDto ToDto() => new(Id, BookCopyId, ReaderId, IssuedOn, ReturnedOn);

    public static Loan FromDto(LoanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentException("Ідентифікатор обов'язковий", nameof(dto.Id));

        if (string.IsNullOrWhiteSpace(dto.BookCopyId))
            throw new ArgumentException("Ідентифікатор примірника обов'язковий", nameof(dto.BookCopyId));

        if (string.IsNullOrWhiteSpace(dto.ReaderId))
            throw new ArgumentException("Ідентифікатор читача обов'язковий", nameof(dto.ReaderId));

        if (dto.ReturnedOn.HasValue && dto.ReturnedOn.Value < dto.IssuedOn)
            throw new ArgumentOutOfRangeException(
                nameof(dto.ReturnedOn),
                dto.ReturnedOn.Value,
                "Дата повернення не може бути ранішою за дату видачі");

        return new Loan(dto.Id.Trim(), dto.BookCopyId.Trim(), dto.ReaderId.Trim(), dto.IssuedOn, dto.ReturnedOn);
    }

    public override string ToString() =>
        $"Видача {Id}: Примірник {BookCopyId} -> Читач {ReaderId} | " +
        (IsClosed ? $"Закрито {ReturnedOn:yyyy-MM-dd}" : $"Відкрито {IssuedOn:yyyy-MM-dd}");
}