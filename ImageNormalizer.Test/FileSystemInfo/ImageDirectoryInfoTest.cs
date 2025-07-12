using System.IO;
using NSubstitute;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;

namespace ImageNormalizer.Test.FileSystemInfo;

[IntegrationTest]
public class ImageDirectoryInfoTest : TestBase
{
    public ImageDirectoryInfoTest()
    {
		_imageFileExtensionService = new ImageFileExtensionService();

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();

		_logger = Substitute.For<ILogger>();

		IImageTransformer imageTransformer = new ImageTransformer(
			imageResizeCalculator, _logger);
		_imageNormalizerService = new ImageNormalizerService(imageTransformer);
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

		var imageDirectoryInfo = new ImageDirectoryInfo(
			_imageFileExtensionService,
			_imageNormalizerService,
			_directoryService,
			_logger,
			arguments);

		// Act
		imageDirectoryInfo.BuildFileSystemInfo();
		imageDirectoryInfo.NormalizeFileSystemInfo();

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
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	#endregion
}
