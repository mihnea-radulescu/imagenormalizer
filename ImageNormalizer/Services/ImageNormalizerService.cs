using System.IO;
using ImageNormalizer.Adapters;
using ImageNormalizer.Logger;

namespace ImageNormalizer.Services;

public class ImageNormalizerService : IImageNormalizerService
{
	public ImageNormalizerService(IImageTransformer imageTransformer, ILogger logger)
	{
		_imageTransformer = imageTransformer;
		_logger = logger;
	}

	public Stream? NormalizeImage(Stream inputImageDataStream, Arguments arguments)
	{
		Stream? outputImageDataStream = default;

		try
		{
			outputImageDataStream = _imageTransformer.TransformImage(
				inputImageDataStream, arguments);
		}
		catch
		{
			_logger.Error(@$"Could not normalize image ""{arguments.InputPath}"".");
		}

		return outputImageDataStream;
	}

	#region Private

	private readonly IImageTransformer _imageTransformer;
	private readonly ILogger _logger;

	#endregion
}
