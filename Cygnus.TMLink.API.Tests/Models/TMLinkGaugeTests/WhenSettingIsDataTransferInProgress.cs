using Cygnus.TMLink.API.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests
{
    [TestFixture]
    internal class WhenSettingIsDataTransferInProgress
    {
        [Test]
        public void ShouldUpdateIsDataTransferInProgress([Values] bool isDataTransferInProgress)
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            tb.Observer.Setup(o => o.OnPropertiesUpdated(It.Is<TMLinkGauge>(g => g == sut)));

            // Act
            sut.IsDataTransferInProgress = isDataTransferInProgress;

            // Assert
            sut.IsDataTransferInProgress.ShouldBe(isDataTransferInProgress);
        }

        [Test]
        public void WhenValueChanges_NotifiesObserver()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            tb.Observer.Setup(o => o.OnPropertiesUpdated(It.Is<TMLinkGauge>(g => g == sut)));

            // Act
            sut.IsDataTransferInProgress = true;

            // Assert
            tb.Observer.Verify(o => o.OnPropertiesUpdated(It.Is<TMLinkGauge>(g => g == sut)), Times.Once);
        }

        [Test]
        public void WhenSetToSameValue_DoesNotNotifyObserver()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            sut.IsDataTransferInProgress = false;
            tb.Observer.Invocations.Clear();

            // Act
            sut.IsDataTransferInProgress = false;

            // Assert
            tb.Observer.Verify(o => o.OnPropertiesUpdated(It.IsAny<TMLinkGauge>()), Times.Never);
        }
    }
}