# Cygnus.Services

### Measurement display
The thickness and velocity values in the measurements handed out by the APIs are raw values that need to be converted to be displayed in a user friendly way.

Note that some records on the same gauge may have been recorded in metric others in imperial, and the end user may have a preference for the units they want to see in your app.

These conversions can be handled by the `MeasurementConverter` in the Cygnus.Services namespace, which can be registered in DI as an implementation of the `IMeasurementConverter` interface by calling
``` C#
services.AddCygnusServices();
```
The `MeasurementConverter` uses the settings from an `IMeasurementDisplaySettingsService` to determine how to convert the raw values for display. It is used to manage the 2 settings related to how measurements are displayed, 
- Units, ie Metric or imperial
- Resolution, ie Low, Medium, High, 
which match the settings on the gauge.

Clients should implement IMeasurementDisplaySettingsService with a class registered in DI, in order to control the results from the MeasurementConverter.

### Examples
In a view model that needs to display the live measurement from a TM-Link BLE gauge, you can inject an IMeasurementConverter and use it to convert the double values from the live measurement to display values, like this:
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

In a view model that needs to display a live measurement from a COM port connected gauge, you can inject an IMeasurementConverter and use it to convert the uint values from the live measurement to display values, like this:
``` C#
        public void UpdateLiveMeasurement(LiveMeasurement liveMeasurement)
        {
            LiveMeasurement = new LiveMeasurementViewModel
            {
                Thickness = _measurementConverter.GetDisplayedThickness(liveMeasurement.Thickness, liveMeasurement.Velocity, liveMeasurement.Units),
                Velocity = _measurementConverter.GetDisplayedVelocity(liveMeasurement.Velocity, liveMeasurement.Units),
            };
        }
```
