using Cygnus.BLE.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests;
internal class WhenSetDeviceIsCalled
{
    [Test]
    public void ShouldAddThisInstanceAsAnObserver()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var device = testBed.CreateDevice();

        // Act
        sut.SetDevice(device.Object);

        // Assert
        device.Verify(d => d.AddObserver(sut), Times.Once);
    }

    [Test]
    public void ShouldSetNameToDeviceName()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var device = testBed.CreateDevice();

        // Act
        sut.SetDevice(device.Object);

        // Assert
        sut.Name.ShouldBe(device.Object.Name);
    }

    [Test]
    public void ShouldDeviceIdentifierToDeviceId()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var device = testBed.CreateDevice();

        // Act
        sut.SetDevice(device.Object);

        // Assert
        sut.DeviceIdentifier.ShouldBe(device.Object.Id);
    }
}
