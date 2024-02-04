﻿using System;
using System.IO;
using Xunit;
using ImageNormalizer.Adapters;
using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.ImageResizing;
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

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageTransformer imageTransformer = new ImageSharpImageTransformer(
			imageResizeCalculator);

		_imageNormalizerService = new ImageNormalizerService(imageTransformer);

		_logger = new NullLogger();
	}

    [Fact]
	public void ValidInputStructure_CreatesExpectedOutputStructure()
	{
		// Arrange
		var inputDirectory = TestDataPath;
		var outputDirectory = $"{TestDataPath}_Output";
		const int outputMaximumImageSize = 960;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		var imageDirectoryInfo = new ImageDirectoryInfo(
			_imageFileExtensionService,
			_imageNormalizerService,
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

	[Fact]
	public void Constructor_InputPathEqualsOutputPath_ThrowsExpectedException()
	{
		// Arrange
		var inputDirectory = TestDataPath;
		var outputDirectory = TestDataPath;
		const int outputMaximumImageSize = 960;
		const int outputImageQuality = 80;

		var arguments = new Arguments(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);

		// Act and Assert
		Assert.Throws<ArgumentException>(() =>
		{
			var imageDirectoryInfo = new ImageDirectoryInfo(
				_imageFileExtensionService,
				_imageNormalizerService,
				_logger,
				arguments);
		});
	}

	#region Private

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly ILogger _logger;

	#endregion
}
