using Shouldly.ShouldlyExtensionMethods;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests
{
    [TestFixture]
    internal class WhenGettingSupportedFeatures
    {
        [Test]
        public void IncludesCanCancelRecordTransfer()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT();

            // Act
            var features = sut.SupportedFeatures;

            // Assert
            features.ShouldHaveFlag(Cygnus.Models.GaugeFeatures.CanCancelRecordTransfer);
        }

        [Test]
        public void IncludesCanDeleteRecords()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT();

            // Act
            var features = sut.SupportedFeatures;

            // Assert
            features.ShouldHaveFlag(Cygnus.Models.GaugeFeatures.CanDeleteRecords);
        }

        [Test]
        public void IncludesHasAScanCapability()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT();

            // Act
            var features = sut.SupportedFeatures;

            // Assert
            features.ShouldHaveFlag(Cygnus.Models.GaugeFeatures.HasAScanCapability);
        }

        [Test]
        public void IncludesHasBScanCapability()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT();

            // Act
            var features = sut.SupportedFeatures;

            // Assert
            features.ShouldHaveFlag(Cygnus.Models.GaugeFeatures.HasBScanCapability);
        }

        [Test]
        public void IncludesSendsBatteryLevel()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT();

            // Act
            var features = sut.SupportedFeatures;

            // Assert
            features.ShouldHaveFlag(Cygnus.Models.GaugeFeatures.SendsBatteryLevel);
        }
    }
}