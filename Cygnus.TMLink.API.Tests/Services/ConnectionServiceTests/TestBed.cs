using Cygnus.TMLink.API.Services;
using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Cygnus.Interfaces;

namespace Cygnus.TMLink.API.Tests.Services.ConnectionServiceTests;
internal class TestBed
{
    private bool _gaugesConnect;
    private bool _gaugesHaveSerialNumber;

    public TestBed()
    {
        GaugeFactory = () => CreateMockGauge(_gaugesConnect, _gaugesHaveSerialNumber);
    }

    public ILogger<ITMLinkConnectionService> Logger { get; set; } = Mock.Of<ILogger<ITMLinkConnectionService>>();
    public Mock<IPlatformService> PlatformService { get; set; } = new Mock<IPlatformService>(MockBehavior.Strict);
    public Mock<ITMLinkDeviceDiscoverer> DeviceDiscoverer { get; set; } = new Mock<ITMLinkDeviceDiscoverer>(MockBehavior.Strict);
    public Mock<IConnectionObserver> Observer { get; private set; }
    public List<Mock<ITMLinkGauge>> Gauges { get; set; } = [];
    public Func<ITMLinkGauge> GaugeFactory { get; set; }

    internal ConnectionService CreateSUT(bool configureObserver = false, bool bluetoothEnabled = true, bool gaugesConnect = true, bool gaugesHaveSerialNumber = true)
    {
        _gaugesConnect = gaugesConnect;
        _gaugesHaveSerialNumber = gaugesHaveSerialNumber;
        ConnectionService connectionService = new(Logger, PlatformService?.Object, DeviceDiscoverer?.Object, GaugeFactory);
        if (configureObserver)
        {
            Observer = new Mock<IConnectionObserver>(MockBehavior.Strict);
            connectionService.AddObserver(Observer.Object);
        }

        PlatformService?.Setup(p => p.CheckBluetoothConfiguration()).ReturnsAsync(bluetoothEnabled);
        if (!bluetoothEnabled)
        {
            Observer?.SetupSet(p => p.ConnectionState = Cygnus.Models.ConnectionState.Errored);
            Observer?.Setup(p => p.AddConnectionMessage("For TM-Link gauges, enable bluetooth and give the app the required permissions"));
        }

        return connectionService;
    }

    private ITMLinkGauge CreateMockGauge(bool gaugeConnected, bool gaugesHasSerialNumber)
    {
        var mockGauge = new Mock<ITMLinkGauge>(MockBehavior.Strict);
        mockGauge.Setup(g => g.Connect()).ReturnsAsync(gaugeConnected);
        mockGauge.Setup(g => g.SetDevice(It.IsAny<ITMLinkDevice>()));
        mockGauge.SetupGet(g => g.Name).Returns(mockGauge.ToString());
        mockGauge.SetupGet(q => q.DeviceIdentifier).Returns(Guid.NewGuid().ToString());
        mockGauge.SetupGet(q => q.SerialNumber).Returns( gaugesHasSerialNumber ? (uint)Random.Shared.Next(1,int.MaxValue) : 0);
        Gauges.Add(mockGauge);
        return mockGauge.Object;
    }
}
