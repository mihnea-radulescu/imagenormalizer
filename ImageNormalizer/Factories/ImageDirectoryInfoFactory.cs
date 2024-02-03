using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.Factories;

public class ImageDirectoryInfoFactory : IImageDirectoryInfoFactory
{
	public ImageDirectoryInfoFactory(
		IImageFileExtensionService imageFileExtensionService,
		IImageNormalizerService imageNormalizerService,
		ILogger logger)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;
		_logger = logger;
	}

	public ImageDirectoryInfo Create(
		string inputDirectory, string outputDirectory, int outputImageQuality)
		=> new ImageDirectoryInfo(
			_imageFileExtensionService,
			_imageNormalizerService,
			_logger,
			inputDirectory,
			outputDirectory,
			outputImageQuality);

	#region Private

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly ILogger _logger;

	#endregion
}
