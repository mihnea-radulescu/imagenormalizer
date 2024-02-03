using ImageNormalizer.Adapters;

namespace ImageNormalizer.Factories;

public class ImageSharpTransformImageFactory : ITransformImageFactory
{
    public ITransformImage GetTransformImage(
		string inputFileName, string outputFileName, int outputImageQuality)
		=> new ImageSharpTransformImage(inputFileName, outputFileName, outputImageQuality);
}
