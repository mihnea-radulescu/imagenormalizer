using ImageNormalizer.Services;

namespace ImageNormalizer.FileSystemInfo;

public class ImageFileInfo : IImageFileSystemInfo
{
	public ImageFileInfo(
		IImageNormalizerService imageNormalizerService,
		Arguments arguments)
	{
		_imageNormalizerService = imageNormalizerService;

		_arguments = arguments;
	}

	public void BuildFileSystemInfo()
	{
	}

	public void NormalizeFileSystemInfo()
	{
		_imageNormalizerService.NormalizeImage(_arguments);
	}

	#region Private

	private readonly IImageNormalizerService _imageNormalizerService;

	private readonly Arguments _arguments;

	#endregion
}
