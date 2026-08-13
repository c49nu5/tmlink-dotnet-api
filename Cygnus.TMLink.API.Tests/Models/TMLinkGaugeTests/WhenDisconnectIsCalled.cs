using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDisconnectIsCalled
{
    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDisconnectOnConnectedDevice()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        
        // Act
        sut.Disconnect();

        // Assert
        testBed.ConnectedDevice.Verify(d => d.Disconnect(), Times.Once);
    }
}
