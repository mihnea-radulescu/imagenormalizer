using NSubstitute;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;
using ImageNormalizer.ImageFileSystem;
using ImageNormalizer.Services;

namespace ImageNormalizer.Test.ImageFileSystem;

[IntegrationTest]
public class ImageFileTest : TestBase
{
	public ImageFileTest()
	{
		_logger = Substitute.For<ILogger>();

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageTransformer imageTransformer = new ImageTransformer(
			imageResizeCalculator);

		_imageDataService = new ImageDataService(_logger);
		_imageNormalizerService = new ImageNormalizerService(imageTransformer, _logger);
	}

	[InlineData("Landscape.jpg", "Landscape_normalized.jpg", 3840, 80, 16)]
	[InlineData("Portrait.jpg", "Portrait_normalized.jpg", 3840, 80, 16)]
	[Theory]
	public void SaveTransformedImage_ValidInputImage_SavesOutputImage(
		string inputFileName,
		string outputFileName,
		int outputMaximumImageSize,
		int outputImageQuality,
		int maxDegreeOfParallelism)
	{
		// Arrange
		var inputFilePath = GetTestFilePath(inputFileName);
		var outputFilePath = GetTestFilePath(outputFileName);

		var arguments = new Arguments(
			inputFilePath,
			outputFilePath,
			outputMaximumImageSize,
			outputImageQuality,
			maxDegreeOfParallelism);

		var imageFile = new ImageFile(_imageDataService, _imageNormalizerService, arguments);

		// Act
		imageFile.ReadImageFromDisc();
		imageFile.NormalizeImage();
		imageFile.WriteImageToDisc();

		// Assert
		Assert.True(ExistsOutputFile(outputFilePath));

		// Tear-down
		DeleteOutputFile(outputFilePath);
	}

	[Fact]
	public void SaveTransformedImage_InvalidInputImage_LogsException()
	{
		// Arrange
		var inputFilePath = GetTestFilePath("InvalidImage.txt");
		var outputFilePath = GetTestFilePath("InvalidImage_normalized.txt");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;
		const int maxDegreeOfParallelism = 16;

		var arguments = new Arguments(
			inputFilePath,
			outputFilePath,
			outputMaximumImageSize,
			outputImageQuality,
			maxDegreeOfParallelism);

		var imageFile = new ImageFile(_imageDataService, _imageNormalizerService, arguments);

		// Act
		imageFile.ReadImageFromDisc();
		imageFile.NormalizeImage();
		imageFile.WriteImageToDisc();

		// Assert
		_logger.Received(1).Error(Arg.Any<string>());
	}

	[Fact]
	public void SaveTransformedImage_NotFoundInputImage_LogsException()
	{
		// Arrange
		var inputFilePath = GetTestFilePath("NotFoundImage.txt");
		var outputFilePath = GetTestFilePath("NotFoundImage_normalized.txt");
		const int outputMaximumImageSize = 3840;
		const int outputImageQuality = 80;
		const int maxDegreeOfParallelism = 16;

		var arguments = new Arguments(
			inputFilePath,
			outputFilePath,
			outputMaximumImageSize,
			outputImageQuality,
			maxDegreeOfParallelism);

		var imageFile = new ImageFile(_imageDataService, _imageNormalizerService, arguments);

		// Act
		imageFile.ReadImageFromDisc();
		imageFile.NormalizeImage();
		imageFile.WriteImageToDisc();

		// Assert
		_logger.Received(1).Error(Arg.Any<string>());
	}

	#region Private

	private readonly ILogger _logger;

	private readonly IImageDataService _imageDataService;
	private readonly IImageNormalizerService _imageNormalizerService;

	#endregion
}
