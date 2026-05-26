using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Services;
using Cygnus.BLE.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class TestBed
{
    private bool _gaugesConnect;
    private bool _gaugesHaveSerialNumber;

    public TestBed()
    {
        GaugeFactory = () => CreateMockGauge(_gaugesConnect, _gaugesHaveSerialNumber);
    }

    public ILogger<IConnectionService> Logger { get; set; } = Mock.Of<ILogger<IConnectionService>>();
    public Mock<IPlatformService> PlatformService { get; set; } = new Mock<IPlatformService>(MockBehavior.Strict);
    public Mock<IGaugeDiscoverer> GaugeDiscoverer { get; set; } = new Mock<IGaugeDiscoverer>(MockBehavior.Strict);
    public Mock<IConnectionMonitor> Observer { get; private set; }
    public List<Mock<IBLEGaugeInternal>> Gauges { get; set; } = [];
    public Func<IBLEGaugeInternal> GaugeFactory { get; set; }

    internal ConnectionService CreateSUT(bool configureObserver = false, bool bluetoothEnabled = true, bool gaugesConnect = true, bool gaugesHaveSerialNumber = true)
    {
        _gaugesConnect = gaugesConnect;
        _gaugesHaveSerialNumber = gaugesHaveSerialNumber;
        ConnectionService connectionService = new(Logger, PlatformService?.Object, GaugeDiscoverer?.Object, GaugeFactory);
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

    private IBLEGaugeInternal CreateMockGauge(bool gaugeConnected, bool gaugesHasSerialNumber)
    {
        var mockGauge = new Mock<IBLEGaugeInternal>(MockBehavior.Strict);
        mockGauge.Setup(g => g.Connect()).ReturnsAsync(gaugeConnected);
        mockGauge.Setup(g => g.SetDevice(It.IsAny<IBLEDevice>()));
        mockGauge.SetupGet(g => g.Name).Returns(mockGauge.ToString());
        mockGauge.SetupGet(q => q.DeviceIdentifier).Returns(Guid.NewGuid().ToString());
        mockGauge.SetupGet(q => q.SerialNumber).Returns( gaugesHasSerialNumber ? Guid.NewGuid().ToString() : null);
        Gauges.Add(mockGauge);
        return mockGauge.Object;
    }
}
