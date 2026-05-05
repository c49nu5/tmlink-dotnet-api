using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenConstructed
{
    [Test]
    public void ShouldReturnAnInstance()
    {
        // Arrange
        var testBed = new TestBed();

        // Act
        var sut = testBed.CreateSUT();

        // Assert
        sut.ShouldNotBeNull();
    }

    [Test]
    public void ShouldThrowIfMeasurementSettingsServiceIsNull()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.MeasurementSettingsService = null;

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }
}
