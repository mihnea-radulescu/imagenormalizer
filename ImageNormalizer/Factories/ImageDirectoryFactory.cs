using System.Threading;
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
		ILogger logger,
		CancellationTokenSource cancellationTokenSource)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageDataService = imageDataService;
		_imageNormalizerService = imageNormalizerService;
		_directoryService = directoryService;
		_logger = logger;

		_cancellationTokenSource = cancellationTokenSource;
	}

	public IImageDirectory Create(Arguments arguments)
		=> new ImageDirectory(
			_imageFileExtensionService,
			_imageDataService,
			_imageNormalizerService,
			_directoryService,
			_logger,
			arguments,
			_cancellationTokenSource);

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageDataService _imageDataService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	private readonly CancellationTokenSource _cancellationTokenSource;
}
