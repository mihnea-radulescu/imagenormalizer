using Xunit;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.ImageResizing;

[UnitTest]
public class ImageResizeCalculatorTest
{
    public ImageResizeCalculatorTest()
    {
        _imageResizeCalculator = new ImageResizeCalculator();
    }

    [Fact]
    public void ShouldResize_ImageMaxSizeBelowResizeThreshold_ReturnsFalse()
    {
        // Arrange
        var imageSize = new ImageSize(1920, 1080);
        var arguments = new Arguments("a", "b", 3840, 0);

        // Act
        var shouldResize = _imageResizeCalculator.ShouldResize(imageSize, arguments);

        // Assert
        Assert.False(shouldResize);
    }

	[Fact]
	public void ShouldResize_ImageWidthAboveResizeThreshold_ReturnsTrue()
	{
		// Arrange
		var imageSize = new ImageSize(4000, 1080);
		var arguments = new Arguments("a", "b", 3840, 0);

		// Act
		var shouldResize = _imageResizeCalculator.ShouldResize(imageSize, arguments);

		// Assert
		Assert.True(shouldResize);
	}

	[Fact]
	public void ShouldResize_ImageHeightAboveResizeThreshold_ReturnsTrue()
	{
		// Arrange
		var imageSize = new ImageSize(1920, 4000);
		var arguments = new Arguments("a", "b", 3840, 0);

		// Act
		var shouldResize = _imageResizeCalculator.ShouldResize(imageSize, arguments);

		// Assert
		Assert.True(shouldResize);
	}

	[Fact]
	public void GetResizedImageSize_ImageMaxSizeBelowResizeThreshold_ReturnsInitialImageSize()
	{
		// Arrange
		var imageSize = new ImageSize(1920, 1080);
		var arguments = new Arguments("a", "b", 3840, 0);

		// Act
		var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(imageSize, arguments);

		// Assert
		Assert.Equal(1920, resizedImageSize.Width);
		Assert.Equal(1080, resizedImageSize.Height);
	}

	[InlineData(3840, 1080, 1920, 1920, 540)]
	[InlineData(1080, 3840, 1920, 540, 1920)]
	[InlineData(1920, 2160, 1080, 960, 1080)]
	[InlineData(2160, 1920, 1080, 1080, 960)]
	[InlineData(1920, 1080, 600, 600, 337)]
	[InlineData(1080, 1920, 600, 337, 600)]
	[Theory]
	public void GetResizedImageSize_ImageMaxSizeAboveResizeThreshold_ReturnsExpectedImageSize(
		int imageWidth,
		int imageHeight,
		int outputMaximumImageSize,
		int resizedImageWidth,
		int resizedImageHeight)
	{
		// Arrange
		var imageSize = new ImageSize(imageWidth, imageHeight);
		var arguments = new Arguments("a", "b", outputMaximumImageSize, 0);

		// Act
		var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(imageSize, arguments);

		// Assert
		Assert.Equal(resizedImageWidth, resizedImageSize.Width);
		Assert.Equal(resizedImageHeight, resizedImageSize.Height);
	}

	#region Private

	private readonly ImageResizeCalculator _imageResizeCalculator;

    #endregion
}
