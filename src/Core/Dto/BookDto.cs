namespace Core.Dto;

public record BookDto(
    string Id,
    string Isbn,
    string Title,
    int Year,
    string? Author = null,
    int Copies = 1,
    string? Genre = null);