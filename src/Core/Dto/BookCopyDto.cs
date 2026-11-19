namespace Core.Dto;

public sealed record BookCopyDto(
    string Id,
    string Isbn,
    string Title,
    bool IsIssued,
    string? Author = null,
    int Year = 0,
    int Copies = 1,
    string? Genre = null);