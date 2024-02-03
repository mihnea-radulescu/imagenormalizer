using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.FileSystemInfo;

public class ImageFileInfo : FileSystemInfoBase
{
	public ImageFileInfo(
		IImageNormalizerService imageNormalizerService,
		ILogger logger,
		string inputPath,
		string outputPath,
		int outputImageQuality)
		: base(logger, inputPath, outputPath, outputImageQuality)
	{
		_imageNormalizerService = imageNormalizerService;
	}

	#region Protected

	protected override void BuildFileSystemInfoSpecific()
	{
	}

	protected override void NormalizeFileSystemInfoSpecific()
	{
		_imageNormalizerService.NormalizeImage(
			InputPath, OutputPath, OutputImageQuality);
	}

	#endregion

	#region Private

	private readonly IImageNormalizerService _imageNormalizerService;

	#endregion
}
