using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.Metrics;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class TestBed
{
    public ILogger<IConnectionService> Logger { get; set; } = Mock.Of<ILogger<IConnectionService>>();
    public Mock<IPlatformService> PlatformService { get; set; } = new Mock<IPlatformService>(MockBehavior.Strict);
    public Mock<IGaugeDiscoverer> GaugeDiscoverer { get; set; } = new Mock<IGaugeDiscoverer>(MockBehavior.Strict);
    public Mock<IConnectionMonitor> Observer { get; private set; }

    internal ConnectionService CreateSUT(bool configureObserver = false, bool bluetoothEnabled = true)
    {
        ConnectionService connectionService = new(Logger, PlatformService?.Object, GaugeDiscoverer?.Object);
        if (configureObserver)
        {
            Observer = new Mock<IConnectionMonitor>(MockBehavior.Strict);
            connectionService.AddObserver(Observer.Object);
        }

        PlatformService?.Setup(p => p.CheckBluetoothConfiguration()).ReturnsAsync(bluetoothEnabled);
        if (!bluetoothEnabled)
        {
            PlatformService?.Setup(p => p.ShowMessage("Please enable bluetooth and give the app the required permissions", "")).Returns(Task.CompletedTask);
        }

        return connectionService;
    }
}
