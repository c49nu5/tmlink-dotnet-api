using Cygnus.Interfaces;
using Moq;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenUnsubscribeToLiveUpdatesIsCalled
{
    [Test]
    public async Task ShouldDelegateToProtobufChannel()
    {
        // Arrange
        var tb = new TestBed();
        var sut = await tb.CreateConnectedSUT();
        var observer = Mock.Of<ILiveMeasurementObserver>();

        tb.Protobuf1Channel.Setup(p => p.RemoveObserver(observer));

        // Act
        sut.UnsubscribeFromLiveUpdates(observer);

        // Assert
        tb.Protobuf1Channel.Verify(p => p.RemoveObserver(observer), Times.Once);
    }
    }
