using ImageNormalizer.Attributes;
using ImageNormalizer.Factories;

namespace ImageNormalizer.Services;

[StatelessService]
public class ImageNormalizerService : IImageNormalizerService
{
    public ImageNormalizerService(ITransformImageFactory transformImageFactory)
    {
        _transformImageFactory = transformImageFactory;
    }

    public void NormalizeImage(string inputFilePath, string outputFilePath)
    {
        using (var transformImage = _transformImageFactory.GetTransformImage(
            inputFilePath, outputFilePath))
        {
            transformImage.SaveTransformedImage();
        }
    }

	#region Private

	private readonly ITransformImageFactory _transformImageFactory;

    #endregion
}
