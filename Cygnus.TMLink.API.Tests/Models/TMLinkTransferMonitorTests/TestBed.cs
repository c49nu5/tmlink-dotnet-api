using Cygnus.Interfaces;
using Cygnus.TMLink.API.Models;
using Moq;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkTransferMonitorTests;

internal class TestBed
{
    public Mock<IFileTransferRequest> TransferRequest { get; set; } = new Mock<IFileTransferRequest>(MockBehavior.Strict);

    public List<bool> ProgressUpdates { get; } = [];

    internal TMLinkTransferMonitor CreateSUT()
    {
        TMLinkTransferMonitor monitor = new(ProgressUpdater, TransferRequest?.Object);

        return monitor;
    }

    private void ProgressUpdater(bool obj)
    {
        ProgressUpdates.Add(obj);
    }
}
