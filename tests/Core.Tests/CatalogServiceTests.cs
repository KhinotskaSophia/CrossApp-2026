using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;
using Xunit;

namespace Core.Tests;

public class CatalogServiceTests
{
    [Fact]
    public void Issue_AlreadyIssuedCopy_ThrowsInvalidOperationException_AndKeepsState()
    {
        var store = new InMemoryBookStore(TestData.Items());
        var service = new LendingService(store);

        var copy = store.GetById("T-001")!;
        service.IssueCopy(copy.Id, "Reader-1", DateTime.Now.AddDays(7));

        Assert.Throws<InvalidOperationException>(() => service.IssueCopy(copy.Id, "Reader-2", DateTime.Now.AddDays(7)));
        Assert.True(store.GetById("T-001")!.IsIssued);
    }

    [Fact]
    public void IssueCopy_RaisesChangedEventExactlyOnce()
    {
        var store = new InMemoryBookStore(TestData.Items());
        var service = new LendingService(store);
        var raisedEvents = new List<LibraryChangedEventArgs>();

        service.Changed += (_, e) => raisedEvents.Add(e);
        service.IssueCopy("T-002", "Reader-1", DateTime.Now.AddDays(7));

        Assert.Single(raisedEvents);
        Assert.Equal(LibraryEventKind.CopyIssued, raisedEvents[0].Kind);
        Assert.Equal("T-002", raisedEvents[0].CopyId);
    }

    [Fact]
    public void Return_NotIssuedCopy_ThrowsInvalidOperationException()
    {
        var store = new InMemoryBookStore(TestData.Items());
        var service = new LendingService(store);

        Assert.Throws<InvalidOperationException>(() => service.ReturnCopy("T-003"));
    }
}