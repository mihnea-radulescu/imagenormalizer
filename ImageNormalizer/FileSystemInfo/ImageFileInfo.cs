using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.FileSystemInfo;

public class ImageFileInfo : FileSystemInfoBase
{
	public ImageFileInfo(
		IImageNormalizerService imageNormalizerService,
		ILogger logger,
		Arguments arguments)
		: base(logger, arguments)
	{
		_imageNormalizerService = imageNormalizerService;
	}

	#region Protected

	protected override void BuildFileSystemInfoSpecific()
	{
	}

	protected override void NormalizeFileSystemInfoSpecific()
	{
		_imageNormalizerService.NormalizeImage(Arguments);
	}

	#endregion

	#region Private

	private readonly IImageNormalizerService _imageNormalizerService;

	#endregion
}
