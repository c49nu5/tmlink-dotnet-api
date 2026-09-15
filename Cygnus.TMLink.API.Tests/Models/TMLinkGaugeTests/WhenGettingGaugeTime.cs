using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests
{
    [TestFixture]
    internal class WhenGettingGaugeTime
    {
        [Test]
        public void ShouldReturnNull()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT();

            // Act
            var features = sut.GaugeTime;

            // Assert
            features.ShouldBeNull();
        }

    }
}