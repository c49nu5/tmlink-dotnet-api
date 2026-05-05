using Cygnus.Interfaces;
using Cygnus.Models;
using Moq;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class TestBed
{
    public Mock<IMeasurementDisplaySettingsService> MeasurementSettingsService { get; set; } = new Mock<IMeasurementDisplaySettingsService>(MockBehavior.Strict);

    internal MeasurementConverter CreateSUT(
        MeasurementUnits units = MeasurementUnits.Default, 
        MeasurementResolution resolution = MeasurementResolution.Default)
    {
        MeasurementSettingsService?.SetupGet(m => m.Units).Returns(units);
        MeasurementSettingsService?.SetupGet(m => m.Resolution).Returns(resolution);
        return new MeasurementConverter(MeasurementSettingsService?.Object);
    }
}
