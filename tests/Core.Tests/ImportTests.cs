using Core.Services;
using Core.Storage;
using Xunit;

namespace Core.Tests;

public class ImportTests
{
    [Fact]
    public async Task ImportAsync_WhenAlreadyCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var service = new LendingService(new InMemoryBookStore());
        var importer = new JsonCatalogImporter();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await service.ImportAsync(importer, "any_path.json", cts.Token)
        );
    }

    [Fact]
    public async Task ImportAsync_InvalidFileFormat_ThrowsDomainException()
    {
        string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        try
        {
            await File.WriteAllTextAsync(tempFile, "{ invalid json content }");

            var service = new LendingService(new InMemoryBookStore());
            var importer = new JsonCatalogImporter();

            await Assert.ThrowsAsync<DomainException>(
                async () => await service.ImportAsync(importer, tempFile)
            );
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}