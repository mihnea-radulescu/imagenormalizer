using System;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.Exceptions;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.Adapters;

[UnitTest]
public class ImageSharpTransformImageTest : TestBase
{
    [InlineData("Landscape.jpg", "Landscape_normalized.jpg", 80)]
    [InlineData("Portrait.jpg", "Portrait_normalized.jpg", 80)]
    [Theory]
    public void SaveTransformedImage_ValidInputImage_SavesOutputImage(
        string inputFileName, string outputFileName, int outputImageQuality)
    {
        // Arrange
        var inputFilePath = GetTestFilePath(inputFileName);
        var outputFilePath = GetTestFilePath(outputFileName);

        // Act
        using (var imageSharpTransformImage = new ImageSharpTransformImage(
			inputFilePath, outputFilePath, outputImageQuality))
        {
			imageSharpTransformImage.SaveTransformedImage();
		}

		// Assert
		Assert.True(ExistsOutputFile(outputFilePath));

		// Tear-down
		DeleteOutputFile(outputFilePath);
	}

    [Fact]
    public void SaveTransformedImage_InvalidInputImage_ThrowsExpectedException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("InvalidImage.txt");
        var outputFilePath = GetTestFilePath("InvalidImage_normalized.txt");
		const int outputImageQuality = 80;

		// Act and Assert
		using (var imageSharpTransformImage = new ImageSharpTransformImage(
			inputFilePath, outputFilePath, outputImageQuality))
		{
			Assert.Throws<TransformImageException>(() =>
				imageSharpTransformImage.SaveTransformedImage());
		}
    }

    [Fact]
    public void SaveTransformedImage_NotFoundInputImage_ThrowsExpectedException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("NotFoundImage.txt");
        var outputFilePath = GetTestFilePath("NotFoundImage_normalized.txt");
		const int outputImageQuality = 80;

		// Act and Assert
		using (var imageSharpTransformImage = new ImageSharpTransformImage(
			inputFilePath, outputFilePath, outputImageQuality))
		{
			Assert.Throws<TransformImageException>(() =>
				imageSharpTransformImage.SaveTransformedImage());
		}
	}

	[InlineData("Landscape.jpg", "Landscape_normalized.jpg", 80)]
	[InlineData("Portrait.jpg", "Portrait_normalized.jpg", 80)]
	[Theory]
	public void SaveTransformedImage_AlreadyDisposedObject_ThrowsExpectedException(
		string inputFileName, string outputFileName, int outputImageQuality)
	{
		// Arrange
		var inputFilePath = GetTestFilePath(inputFileName);
		var outputFilePath = GetTestFilePath(outputFileName);

		// Act
		ImageSharpTransformImage imageSharpTransformImage;

		using (imageSharpTransformImage = new ImageSharpTransformImage(
			inputFilePath, outputFilePath, outputImageQuality))
		{
			imageSharpTransformImage.SaveTransformedImage();
		}

		// Assert
		Assert.Throws<ObjectDisposedException>(() => imageSharpTransformImage.SaveTransformedImage());

		// Tear-down
		DeleteOutputFile(outputFilePath);
	}
}
