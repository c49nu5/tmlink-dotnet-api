using Cygnus.Models;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests
{
    [TestFixture]
    internal class WhenOnLiveMeasurementReceived
    {
        [Test]
        public void WhenFrozen_DoesNotUpdateBatteryLevel()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            sut.BatteryLevel = 100;

            var lm = new LiveMeasurement
            {
                IsFrozen = true,
                BatteryLevel = 42,
                PointIndex = 7
            };

            // Act
            sut.OnLiveMeasurementReceived(lm);

            // Assert
            sut.BatteryLevel.ShouldBe(100u);
        }

        [Test]
        public void WhenFrozen_DoesNotUpdateStatusMessageCount()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            sut.StatusMessageCount = 0;

            var lm = new LiveMeasurement
            {
                IsFrozen = true,
                BatteryLevel = 42,
                PointIndex = 7
            };

            // Act
            sut.OnLiveMeasurementReceived(lm);

            // Assert
            sut.StatusMessageCount.ShouldBe(0u);
        }

        [Test]
        public async Task ShouldSetProbeFromGauge()
        {
            // Arrange
            var tb = new TestBed();
            var sut = await tb.CreateConnectedSUT();
            tb.Observer.Setup(o => o.OnPropertiesUpdated(sut));
            sut.ProbeType = ProbeType.D790;
            var lm = new LiveMeasurement { IsFrozen = false, Probe = ProbeType.S2C_CAWG2 };

            // Act
            sut.OnLiveMeasurementReceived(lm);

            // Assert
            sut.ProbeType.ShouldBe(ProbeType.D790);
        }
    }
}