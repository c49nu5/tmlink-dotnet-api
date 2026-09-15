using Cygnus.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSendMeasurementSetupIsCalled
{
    [Test]
    public async Task WithMeasurementSettingsUpdate_ShouldThrowNotSupportedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.SendMeasurementSetup(Mock.Of<IMeasurementSettingsUpdate>(), Cygnus.Models.MeasurementUnits.Default, Cygnus.Models.MeasurementResolution.Medium);

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }

    [Test]
    public async Task ShouldThrowNotSupportedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.SendMeasurementSetup(Cygnus.Models.MeasurementUnits.Default, Cygnus.Models.MeasurementResolution.Medium);

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }
}