using Xunit;
using ImageNormalizer.Exceptions;
using ImageNormalizer.Factories;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.Services;

[IntegrationTest]
public class ImageNormalizerServiceTest : TestBase
{
    public ImageNormalizerServiceTest()
    {
        ITransformImageFactory transformImageFactory
            = new ImageSharpTransformImageFactory(OutputImageQuality);

        _imageNormalizerService = new ImageNormalizerService(transformImageFactory);
    }

    [InlineData("Landscape.jpg", "Landscape_normalized.jpg")]
    [InlineData("Portrait.jpg", "Portrait_normalized.jpg")]
    [Theory]
    public void NormalizeImage_ValidInputImage_SavesOutputImage(
        string inputFileName, string outputFileName)
    {
        // Arrange
        var inputFilePath = GetTestFilePath(inputFileName);
        var outputFilePath = GetTestFilePath(outputFileName);

		// Act
		_imageNormalizerService.NormalizeImage(inputFilePath, outputFilePath);

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

		// Act and Assert
		Assert.Throws<TransformImageException>(() =>
            _imageNormalizerService.NormalizeImage(inputFilePath, outputFilePath));
    }

    [Fact]
    public void NormalizeImage_NotFoundInputImage_ThrowsExpectedException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("NotFoundImage.txt");
        var outputFilePath = GetTestFilePath("NotFoundImage_normalized.txt");

		// Act and Assert
		Assert.Throws<TransformImageException>(() =>
            _imageNormalizerService.NormalizeImage(inputFilePath, outputFilePath));
    }

    #region Private

    private const int OutputImageQuality = 75;

	private readonly ImageNormalizerService _imageNormalizerService;

	#endregion
}
