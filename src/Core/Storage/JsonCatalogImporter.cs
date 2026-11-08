using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonCatalogImporter : ICatalogImporter
{
    public async Task<IReadOnlyList<BookCopy>> LoadAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested(); // <-- Перевіряємо одразу на вході

        if (!File.Exists(path))
            throw new FileNotFoundException("Файл не знайдено", path);

        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        List<BookCopyDto>? dtos;
        try
        {
            dtos = await JsonSerializer.DeserializeAsync<List<BookCopyDto>>(stream, cancellationToken: ct);
        }
        catch (JsonException ex)
        {
            throw new DomainException($"Некоректний формат JSON файлу {Path.GetFileName(path)}", ex);
        }

        if (dtos is null || dtos.Count == 0)
        {
            throw new DomainException($"Файл {Path.GetFileName(path)} не містить записів або порожній.");
        }

        var result = new List<BookCopy>(dtos.Count);
        for (var i = 0; i < dtos.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                result.Add(BookCopy.FromDto(dtos[i]));
            }
            catch (ArgumentException ex)
            {
                throw new DomainException($"Запис #{i + 1}: {ex.Message}", ex);
            }
        }

        return result;
    }
}