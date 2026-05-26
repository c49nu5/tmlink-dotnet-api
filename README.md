# tmlink-dotnet-api
The Cygnus TM-Link dot-net API that uses BLE (Bluetooth Low Energy) to communicate with gauges.

## TM-Link over BLE
The latest Cygnus 1Ex gauge firmware (from V1.4.xx) supports the TM-Link via BLE communication mode. This allows you to connect to the gauge using a smartphone or computer and read data from it in real-time.

If you want to use the latest Cygnus 1Ex firmware and firmware update utilities please contact <service@cygnus-instruments.com>

The characteristics of the BLE TM-Link Service allow writing commands, receiving notifications and reading responses which contain g-zipped, protobuf messages.  The protobuf messages are defined in the [proto file found here](https://github.com/c49nu5/tmlink-dotnet-api/blob/master/Protos/cyg_tml_api_v1.proto).

The API in this repository provides a .NET interface to connect to the TM-Link service, send commands, and receive data from the gauge. It abstracts away the details of BLE communication and protobuf parsing, allowing you to easily integrate TM-Link functionality into your .NET applications.

## TM-Link API
The API assumes that your code is using Microsoft.Extensions.DependencyInjection, you can register the TM-Link API services in your application's service collection as follows:
``` C#
services.AddSingleton<IPlatformService, PlatformService>();
services.AddBleServices();
```

By default the call to AddBleServices registers an implementation of the IGaugeDiscoverer interface based on the [InTheHand.BluetoothLE package](https://github.com/inthehand/32feet). If you want to use a different BLE library, you can register your own implementation of IGaugeDiscoverer in DI after calling AddBleServices(false) which will prevent the default implementation from being registered.

Clients need to implement `IPlatformService` interface to provide platform-specific functionality such as checking that BLE is configured correctly and displaying messages to the end user. 

An example of these implementations for Maui can be found here
https://github.com/c49nu5/cygnus-tmlink-ble-sdk

### Connecting to gauges
The entry point to the API is the `IConnectionService` interface, which provides methods to discover nearby TM-Link gauges, connect to them, and manage connections. Clients can implement the `IConnectionMonitor` interface to receive updates about discovered gauges and connection status.

An example of using the API in an MVVM application to connect to a TM-Link gauge and read data would look like this:
``` C#
    public partial class ConnectionViewModel : IConnectionMonitor
    {
        private readonly IConnectionService _connectionService;

        public ConnectionViewModel(
            IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _connectionService.AddObserver(this);
            _connectionService.DiscoverGauges();
        }

        public void GaugeDiscovered(IBLEGauge gauge)
        {
            // This instance would usually be stored and displayed to the user to allow them to choose when to connect
            _connectionService.ConnectToGauge(gauge);
        }

        public void GaugeConnected(IBLEGauge? gauge)
        {
            ConnectedGauge = gauge;
        }

        public IBleGauge? ConnectedGauge { get; set; }

        public bool IsScanning { get; set; }
```

The view model implements the `IConnectionMonitor` interface to receive updates about discovered gauges and connection status once it has added itself as an observer to the connection service. The `DiscoverGauges` method initiates a scan for nearby TM-Link gauges, and when a gauge is discovered the code attempts to connect to it. Once connected, the `ConnectedGauge` property is updated with the connected gauge instance, allowing the application to interact with it and read data.

A view model can then use the following methods on the IBleGauge interface to retrieve, delete or create records on the gauge.
``` C#
    Task<List<GaugeRecordSummary>?> GetRecordList();
    Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans);
    Task CancelRecordTransfer();
    Task DeleteAllRecords();
    Task DeleteRecord(IDeleteRequest deleteRequest);
    Task NewRecord(BlankRecord record);
```

### Live updates
If clients wish to receive updates when the live and frozen measurements are updated then the view model can implement the IGaugeMonitor interface and add itself to the IBLEGauge instance as an observer.
``` C#
    public partial class GaugeViewModel : IGaugeMonitor
    {
        private readonly IBleGauge _gauge;

        public GaugeViewModel(
            IBleGauge gauge)
        {
            _gauge = gauge;
            _gauge.AddObserver(this);
        }

        public Task SubscribeToLiveUpdates()
        {
            _gauge.SubscribeToLiveUpdates();
        }

        public void UnsubscribeFromLiveUpdates()
        {
            _gauge.UnsubscribeFromLiveUpdates();
        }

        public void UpdateLiveMeasurement(LiveMeasurement liveMeasurement)
        {
            LiveMeasurement = liveMeasurement;
        }

        public LiveMeasurement? LiveMeasurement { get; set; }
    }

```
In a similar way to the connection view model, the gauge view model can implement the `IGaugeMonitor` interface to receive updates about live measurements after calling SubscribeToLiveUpdates, by adding itself to the IBLEGauge instance as an observer.


### Measurement display
The thickness and velocity values in the measurements handed out by the API are raw values that need to be converted to be displayed in a user friendly way.

In principle that just means dividing the thickness value by 1000 to get the value in mm, or inches.

However note that some records on the same gauge may have been recorded in metric others in imperial, and the end user may have a preference for the units they want to see in your app.

These conversions can be handled by the `MeasurementConverter` in the Cygnus.Services namespace, which is registered in DI as an implementation of the `IMeasurementConverter` interface by calling
``` C#
services.AddCygnusServices();
```
The `MeasurementConverter` uses the settings from an `IMeasurementDisplaySettingsService` to determine how to convert the raw values for display. It is used to manage the 2 settings related to how measurements are displayed, 
- Units, ie Metric or imperial
- Resolution, ie Low, Medium, High, 
which match the settings on the gauge.

Clients should implement IMeasurementDisplaySettingsService with a class registered in DI, in order to control the results from the MeasurementConverter.

``` C#
        public void UpdateLiveMeasurement(LiveMeasurement liveMeasurement)
        {
            LiveMeasurement = new LiveMeasurementViewModel
            {
                Thickness = _measurementConverter.GetDisplayedThickness(liveMeasurement.Thickness, liveMeasurement.Units),
                Velocity = _measurementConverter.GetDisplayedVelocity(liveMeasurement.Velocity, liveMeasurement.Units),
            };
        }
```
