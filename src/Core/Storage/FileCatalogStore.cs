using System.Text.Encodings.Web;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileCatalogStore : IBookStore
{
    private readonly string _filePath;
    private readonly Dictionary<string, BookCopy> _items = new(StringComparer.OrdinalIgnoreCase);
    private bool _loaded;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public FileCatalogStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }

    public IReadOnlyList<BookCopy> List()
    {
        EnsureLoaded();
        return _items.Values.ToList();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        if (!_items.Remove(id))
            return false;

        SaveAtomic();
        return true;
    }

    public BookCopy? GetById(string id)
    {
        EnsureLoaded();
        return _items.GetValueOrDefault(id);
    }

    public void Add(BookCopy copy)
    {
        ArgumentNullException.ThrowIfNull(copy);
        EnsureLoaded();
        _items[copy.Id] = copy;
        SaveAtomic();
    }

    public void Update(BookCopy copy)
    {
        ArgumentNullException.ThrowIfNull(copy);
        EnsureLoaded();
        _items[copy.Id] = copy;
        SaveAtomic();
    }

    public void Export(string? targetPath = null)
    {
        EnsureLoaded();
        var destination = string.IsNullOrWhiteSpace(targetPath) ? _filePath : Path.GetFullPath(targetPath);
        WriteAtomicToPath(destination);
    }

    private void SaveAtomic() => WriteAtomicToPath(_filePath);

    private void WriteAtomicToPath(string destinationPath)
    {
        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var dtos = _items.Values.Select(item => item.ToDto()).ToList();
        var json = JsonSerializer.Serialize(dtos, SerializerOptions);
        var tempPath = destinationPath + ".tmp";

        File.WriteAllText(tempPath, json);
        File.Move(tempPath, destinationPath, overwrite: true);
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;

        if (!File.Exists(_filePath))
        {
            _loaded = true;
            return;
        }

        var content = File.ReadAllText(_filePath);
        List<BookCopyDto>? dtos;
        try
        {
            dtos = JsonSerializer.Deserialize<List<BookCopyDto>>(content, SerializerOptions);
        }
        catch (JsonException ex)
        {
            throw new DomainException($"Некоректний формат JSON у файлі '{Path.GetFileName(_filePath)}'.", ex);
        }

        if (dtos is null)
        {
            throw new DomainException($"Файл '{Path.GetFileName(_filePath)}' не містить списку записів.");
        }

        _items.Clear();
        for (var i = 0; i < dtos.Count; i++)
        {
            try
            {
                var copy = BookCopy.FromDto(dtos[i]);
                _items[copy.Id] = copy;
            }
            catch (ArgumentException ex)
            {
                throw new DomainException($"Запис #{i + 1}: {ex.Message}", ex);
            }
        }

        _loaded = true;
    }
}