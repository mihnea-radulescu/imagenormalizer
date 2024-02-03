using ImageNormalizer.Factories;

namespace ImageNormalizer.Services;

public class ImageNormalizerService : IImageNormalizerService
{
    public ImageNormalizerService(ITransformImageFactory transformImageFactory)
    {
        _transformImageFactory = transformImageFactory;
    }

    public void NormalizeImage(
        string inputFilePath, string outputFilePath, int outputImageQuality)
    {
        using (var transformImage = _transformImageFactory.GetTransformImage(
            inputFilePath, outputFilePath, outputImageQuality))
        {
            transformImage.SaveTransformedImage();
        }
    }

	#region Private

	private readonly ITransformImageFactory _transformImageFactory;

    #endregion
}
