using ImageNormalizer.Adapters;

namespace ImageNormalizer.Factories;

public interface ITransformImageFactory
{
	ITransformImage GetTransformImage(
		string inputFileName, string outputFileName, int outputImageQuality);
}
