using ImageNormalizer.Attributes;
using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.Factories;

[StatelessService]
public class FileSystemInfoFactory : IFileSystemInfoFactory
{
	public FileSystemInfoFactory(
        IImageFileExtensionService imageFileExtensionService,
        IImageNormalizerService imageNormalizerService,
        ILogger logger)
    {
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;
		_logger = logger;
	}

    public IFileSystemInfo GetFileSystemInfo(string inputDirectory, string outputDirectory)
		=> new ImageDirectoryInfo(
			_imageFileExtensionService,
			_imageNormalizerService,
			_logger,
			inputDirectory,
			outputDirectory);

	#region Private

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly ILogger _logger;

	#endregion
}
