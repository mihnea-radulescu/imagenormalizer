using System.IO;
using NSubstitute;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.ImageFileSystem;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.ImageFileSystem;

[IntegrationTest]
public class ImageDirectoryTest : TestBase
{
	public ImageDirectoryTest()
	{
		_logger = Substitute.For<ILogger>();

		_imageFileExtensionService = new ImageFileExtensionService();

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageTransformer imageTransformer = new ImageTransformer(
			imageResizeCalculator);

		_imageDataService = new ImageDataService(_logger);
		_imageNormalizerService = new ImageNormalizerService(imageTransformer, _logger);
		_directoryService = new DirectoryService();
	}

	[Fact]
	public void ValidInputStructure_CreatesExpectedOutputStructure()
	{
		// Arrange
		var inputDirectory = TestDataPath;
		var outputDirectory = $"{TestDataPath}_Output";
		const int outputMaximumImageSize = 960;
		const int outputImageQuality = 80;
		const int maxDegreeOfParallelism = 16;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality, maxDegreeOfParallelism);

		var imageDirectory = new ImageDirectory(
			_imageFileExtensionService,
			_imageDataService,
			_imageNormalizerService,
			_directoryService,
			_logger,
			arguments);

		// Act
		imageDirectory.BuildImageDirectory();
		imageDirectory.NormalizeImages();

		// Assert
		var nestedLandscape1OutputFile = Path.Combine(outputDirectory, "Subfolder1", "Landscape.jpg");
		var nestedLandscape2OutputFile = Path.Combine(outputDirectory, "Subfolder1", "Landscape2.jpg");

		var nestedPortrait1OutputFile = Path.Combine(
			outputDirectory, "Subfolder2", "Subfolder2_1", "Portrait.jpg");
		var nestedPortrait2OutputFile = Path.Combine(
			outputDirectory, "Subfolder2", "Subfolder2_1", "Portrait2.jpg");

		Assert.True(ExistsOutputFile(nestedLandscape1OutputFile));
		Assert.True(ExistsOutputFile(nestedLandscape2OutputFile));

		Assert.True(ExistsOutputFile(nestedPortrait1OutputFile));
		Assert.True(ExistsOutputFile(nestedPortrait2OutputFile));

		// Tear-down
		DeleteOutputDirectory(outputDirectory);
	}

	#region Private

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageDataService _imageDataService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	#endregion
}
