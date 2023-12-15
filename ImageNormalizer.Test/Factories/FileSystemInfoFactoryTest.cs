using System.IO;
using Xunit;
using ImageNormalizer.Factories;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;
using ImageNormalizer.Test.TestTypes;
using ImageNormalizer.Test.TestTypes.Attributes;
using ImageNormalizer.Test.TestTypes.Stubs;

namespace ImageNormalizer.Test.Factories;

[IntegrationTest]
public class FileSystemInfoFactoryTest : TestBase
{
    public FileSystemInfoFactoryTest()
    {
		IImageFileExtensionService imageFileExtensionService = new ImageFileExtensionService();

		ITransformImageFactory transformImageFactory = new ImageSharpTransformImageFactory(
			OutputImageQuality);
		IImageNormalizerService imageNormalizerService = new ImageNormalizerService(
			transformImageFactory);

		ILogger logger = new NullLogger();

		_fileSystemInfoFactory = new FileSystemInfoFactory(
			imageFileExtensionService,
			imageNormalizerService,
			logger);
	}

    [Fact]
	public void ValidInputStructure_CreatesExpectedOutputStructure()
	{
		// Arrange
		var inputDirectory = TestDataPath;
		var outputDirectory = $"{TestDataPath}_Output";

		var fileSystemInfo = _fileSystemInfoFactory.GetFileSystemInfo(inputDirectory, outputDirectory);

		// Act
		fileSystemInfo.BuildFileSystemInfo();
		fileSystemInfo.NormalizeFileSystemInfo();

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

	private const int OutputImageQuality = 75;

	private readonly FileSystemInfoFactory _fileSystemInfoFactory;

	#endregion
}
