using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.Exceptions;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.Adapters;

[UnitTest]
public class ImageSharpImageTransformerTest : TestBase
{
    public ImageSharpImageTransformerTest()
    {
        _imageSharpImageTransformer = new ImageSharpImageTransformer();
    }

    [InlineData("Landscape.jpg", "Landscape_normalized.jpg", 80)]
    [InlineData("Portrait.jpg", "Portrait_normalized.jpg", 80)]
    [Theory]
    public void SaveTransformedImage_ValidInputImage_SavesOutputImage(
        string inputFileName, string outputFileName, int outputImageQuality)
    {
        // Arrange
        var inputFilePath = GetTestFilePath(inputFileName);
        var outputFilePath = GetTestFilePath(outputFileName);

        var arguments = new Arguments(
			inputFilePath, outputFilePath, outputImageQuality);

		// Act
		_imageSharpImageTransformer.TransformImage(arguments);

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

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputImageQuality);

		// Act and Assert
		Assert.Throws<TransformImageException>(() =>
			_imageSharpImageTransformer.TransformImage(arguments));
	}

    [Fact]
    public void SaveTransformedImage_NotFoundInputImage_ThrowsExpectedException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("NotFoundImage.txt");
        var outputFilePath = GetTestFilePath("NotFoundImage_normalized.txt");
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputImageQuality);

		// Act and Assert
		Assert.Throws<TransformImageException>(() =>
			_imageSharpImageTransformer.TransformImage(arguments));
	}

	#region Private

	private readonly ImageSharpImageTransformer _imageSharpImageTransformer;

	#endregion
}