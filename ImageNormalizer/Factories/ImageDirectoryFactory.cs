using ImageNormalizer.ImageFileSystem;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.Factories;

public class ImageDirectoryFactory : IImageDirectoryFactory
{
	public ImageDirectoryFactory(
		IImageFileExtensionService imageFileExtensionService,
		IImageDataService imageDataService,
		IImageNormalizerService imageNormalizerService,
		IDirectoryService directoryService,
		ILogger logger)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageDataService = imageDataService;
		_imageNormalizerService = imageNormalizerService;
		_directoryService = directoryService;
		_logger = logger;
	}

	public IImageDirectory Create(Arguments arguments)
		=> new ImageDirectory(
			_imageFileExtensionService,
			_imageDataService,
			_imageNormalizerService,
			_directoryService,
			_logger,
			arguments);

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageDataService _imageDataService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;
}
