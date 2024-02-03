using ImageNormalizer.Adapters;

namespace ImageNormalizer.Services;

public class ImageNormalizerService : IImageNormalizerService
{
	public ImageNormalizerService(IImageTransformer imageTransformer)
    {
		_imageTransformer = imageTransformer;
	}

    public void NormalizeImage(
        string inputFilePath, string outputFilePath, int outputImageQuality)
    {
        _imageTransformer.TransformImage(inputFilePath, outputFilePath, outputImageQuality);
	}

	#region Private

	private readonly IImageTransformer _imageTransformer;

	#endregion
}
