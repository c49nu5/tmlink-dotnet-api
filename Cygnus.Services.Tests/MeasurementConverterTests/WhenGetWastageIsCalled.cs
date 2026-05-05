
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetWastageIsCalled
{
    [Test]
    [TestCase("10", "15", "5", "", "", .5d)]
    [TestCase("10", "", "", "15", "5", .5d)]
    [TestCase("10", "15", "", "", "5", .5d)]
    [TestCase("10", "", "", "", "", null)]
    [TestCase("", "14", "4", "15", "5", null)]
    [TestCase("10", "5", "5", "", "", 0)]

    public void CorrectValueShouldBeReturned(string displayedThickness, string displayedRef, string displayedMin, string parentRef, string parentMin, double? expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var wastage = sut.GetWastage(displayedThickness, displayedRef, displayedMin, parentRef, parentMin);

        // Assert
        wastage.ShouldBe(expected);
    }

    [Test]
    [TestCase("10", "0", "5", "15", "", .5d)]
    public void AndRefIsEmpty_ShouldUseParentRef(string displayedThickness, string displayedRef, string displayedMin, string parentRef, string parentMin, double? expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        displayedRef = string.Empty;
        // Act
        var wastage = sut.GetWastage(displayedThickness, displayedRef, displayedMin, parentRef, parentMin);

        // Assert
        wastage.ShouldBe(expected);
    }

    [Test]
    [TestCase("10", "15", "0", "", "5", .5d)]
    public void AndMinIsEmpty_ShouldUseParentMin(string displayedThickness, string displayedRef, string displayedMin, string parentRef, string parentMin, double? expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        displayedMin = string.Empty;
        // Act
        var wastage = sut.GetWastage(displayedThickness, displayedRef, displayedMin, parentRef, parentMin);

        // Assert
        wastage.ShouldBe(expected);
    }
}
