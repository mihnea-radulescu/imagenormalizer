using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.Exceptions;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.Services;

[IntegrationTest]
public class ImageNormalizerServiceTest : TestBase
{
    public ImageNormalizerServiceTest()
    {
        IImageTransformer imageTransformer = new ImageSharpImageTransformer();

        _imageNormalizerService = new ImageNormalizerService(imageTransformer);
    }

    [InlineData("Landscape.jpg", "Landscape_normalized.jpg", 80)]
    [InlineData("Portrait.jpg", "Portrait_normalized.jpg", 80)]
    [Theory]
    public void NormalizeImage_ValidInputImage_SavesOutputImage(
        string inputFileName, string outputFileName, int outputImageQuality)
    {
        // Arrange
        var inputFilePath = GetTestFilePath(inputFileName);
        var outputFilePath = GetTestFilePath(outputFileName);

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputImageQuality);

		// Act
		_imageNormalizerService.NormalizeImage(arguments);

		// Assert
		Assert.True(ExistsOutputFile(outputFilePath));

        // Tear-down
		DeleteOutputFile(outputFilePath);
	}

    [Fact]
    public void NormalizeImage_InvalidInputImage_ThrowsExpectedException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("InvalidImage.txt");
        var outputFilePath = GetTestFilePath("InvalidImage_normalized.txt");
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputImageQuality);

		// Act and Assert
		Assert.Throws<TransformImageException>(() =>
            _imageNormalizerService.NormalizeImage(arguments));
    }

    [Fact]
    public void NormalizeImage_NotFoundInputImage_ThrowsExpectedException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("NotFoundImage.txt");
        var outputFilePath = GetTestFilePath("NotFoundImage_normalized.txt");
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputImageQuality);

		// Act and Assert
		Assert.Throws<TransformImageException>(() =>
            _imageNormalizerService.NormalizeImage(arguments));
    }

    #region Private

	private readonly ImageNormalizerService _imageNormalizerService;

	#endregion
}
