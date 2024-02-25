using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.Factories;

public class ImageDirectoryInfoFactory : IImageDirectoryInfoFactory
{
	public ImageDirectoryInfoFactory(
		IImageFileExtensionService imageFileExtensionService,
		IImageNormalizerService imageNormalizerService,
		IDirectoryService directoryService,
		ILogger logger)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;
		_directoryService = directoryService;
		_logger = logger;
	}

	public ImageDirectoryInfo Create(Arguments arguments)
		=> new ImageDirectoryInfo(
			_imageFileExtensionService,
			_imageNormalizerService,
			_directoryService,
			_logger,
			arguments);

	#region Private

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	#endregion
}
