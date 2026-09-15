using Cygnus.Interfaces;
using Moq;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

[TestFixture]
internal class WhenSubscribeToLiveUpdatesIsCalled
{
    [Test]
    public async Task ShouldDelegateToProtobufChannel()
    {
        // Arrange
        var tb = new TestBed();
        var sut = await tb.CreateConnectedSUT();
        var observer = Mock.Of<ILiveMeasurementObserver>();

        tb.Protobuf1Channel.Setup(p => p.AddObserver(observer));

        // Act
        sut.SubscribeToLiveUpdates(observer);

        // Assert
        tb.Protobuf1Channel.Verify(p => p.AddObserver(observer), Times.Once);
    }
}
