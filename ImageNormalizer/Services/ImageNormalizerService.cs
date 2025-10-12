using ImageNormalizer.Adapters;

namespace ImageNormalizer.Services;

public class ImageNormalizerService : IImageNormalizerService
{
	public ImageNormalizerService(IImageTransformer imageTransformer)
	{
		_imageTransformer = imageTransformer;
	}

	public void NormalizeImage(Arguments arguments)
	{
		_imageTransformer.TransformImage(arguments);
	}

	#region Private

	private readonly IImageTransformer _imageTransformer;

	#endregion
}
