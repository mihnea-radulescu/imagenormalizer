using System;
using System.IO;
using Xunit;
using ImageNormalizer.Factories;
using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;
using ImageNormalizer.Test.TestTypes.Stubs;

namespace ImageNormalizer.Test.FileSystemInfo;

[IntegrationTest]
public class ImageDirectoryInfoTest : TestBase
{
    public ImageDirectoryInfoTest()
    {
		_imageFileExtensionService = new ImageFileExtensionService();

		ITransformImageFactory transformImageFactory = new ImageSharpTransformImageFactory(
			OutputImageQuality);
		_imageNormalizerService = new ImageNormalizerService(transformImageFactory);

		_logger = new NullLogger();
	}

    [Fact]
	public void ValidInputStructure_CreatesExpectedOutputStructure()
	{
		// Arrange
		var inputDirectory = TestDataPath;
		var outputDirectory = $"{TestDataPath}_Output";

		var imageDirectoryInfo = new ImageDirectoryInfo(
			_imageFileExtensionService,
			_imageNormalizerService,
			_logger,
			inputDirectory,
			outputDirectory);

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

	[Fact]
	public void Constructor_InputPathEqualsOutputPath_ThrowsExpectedException()
	{
		// Arrange
		var inputDirectory = TestDataPath;
		var outputDirectory = TestDataPath;

		// Act and Assert
		Assert.Throws<ArgumentException>(() =>
		{
			var imageDirectoryInfo = new ImageDirectoryInfo(
				_imageFileExtensionService,
				_imageNormalizerService,
				_logger,
				inputDirectory,
				outputDirectory);
		});
	}

	#region Private

	private const int OutputImageQuality = 80;

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly ILogger _logger;

	#endregion
}
