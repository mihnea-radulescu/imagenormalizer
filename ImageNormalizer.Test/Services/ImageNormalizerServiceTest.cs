using NSubstitute;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.Exceptions;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.Services;

[IntegrationTest]
public class ImageNormalizerServiceTest : TestBase
{
    public ImageNormalizerServiceTest()
    {
        IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();

		_logger = Substitute.For<ILogger>();

		IImageTransformer imageTransformer = new ImageSharpImageTransformer(
            imageResizeCalculator, _logger);

        _imageNormalizerService = new ImageNormalizerService(imageTransformer);
    }

    [InlineData("Landscape.jpg", "Landscape_normalized.jpg", 3840, 80)]
    [InlineData("Portrait.jpg", "Portrait_normalized.jpg", 3840, 80)]
    [Theory]
    public void NormalizeImage_ValidInputImage_SavesOutputImage(
		string inputFileName,
		string outputFileName,
		int outputMaximumImageSize,
		int outputImageQuality)
	{
        // Arrange
        var inputFilePath = GetTestFilePath(inputFileName);
        var outputFilePath = GetTestFilePath(outputFileName);

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputMaximumImageSize, outputImageQuality);

		// Act
		_imageNormalizerService.NormalizeImage(arguments);

		// Assert
		Assert.True(ExistsOutputFile(outputFilePath));

        // Tear-down
		DeleteOutputFile(outputFilePath);
	}

    [Fact]
    public void NormalizeImage_InvalidInputImage_LogsException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("InvalidImage.txt");
        var outputFilePath = GetTestFilePath("InvalidImage_normalized.txt");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputMaximumImageSize, outputImageQuality);

		// Act
		_imageNormalizerService.NormalizeImage(arguments);

		// Assert
		_logger.Received(1).Error(Arg.Any<TransformImageException>());
	}

    [Fact]
    public void NormalizeImage_NotFoundInputImage_LogsException()
    {
        // Arrange
        var inputFilePath = GetTestFilePath("NotFoundImage.txt");
        var outputFilePath = GetTestFilePath("NotFoundImage_normalized.txt");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputFilePath, outputFilePath, outputMaximumImageSize, outputImageQuality);

		// Act
		_imageNormalizerService.NormalizeImage(arguments);

		// Assert
		_logger.Received(1).Error(Arg.Any<TransformImageException>());
	}

	#region Private

	private readonly ILogger _logger;

	private readonly ImageNormalizerService _imageNormalizerService;

	#endregion
}
