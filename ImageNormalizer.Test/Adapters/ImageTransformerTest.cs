using NSubstitute;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.Exceptions;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.Adapters;

[UnitTest]
public class ImageTransformerTest : TestBase
{
	public ImageTransformerTest()
	{
		IImageResizeCalculator imageResizeCalculator = Substitute.For<IImageResizeCalculator>();
		imageResizeCalculator
			.ShouldResize(Arg.Any<ImageSize>(), Arg.Any<Arguments>())
			.Returns(false);

		_logger = Substitute.For<ILogger>();

		_imageTransformer = new ImageTransformer(imageResizeCalculator, _logger);
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

		// Act
		_imageTransformer.TransformImage(arguments);

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

		// Act
		_imageTransformer.TransformImage(arguments);

		// Assert
		_logger.Received(1).Error(Arg.Any<TransformImageException>());
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

		// Act
		_imageTransformer.TransformImage(arguments);

		// Assert
		_logger.Received(1).Error(Arg.Any<TransformImageException>());
	}

	#region Private

	private readonly ILogger _logger;

	private readonly ImageTransformer _imageTransformer;

	#endregion
}
