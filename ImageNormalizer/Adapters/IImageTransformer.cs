namespace ImageNormalizer.Adapters;

public interface IImageTransformer
{
	void TransformImage(
		string inputFileName, string outputFileName, int outputImageQuality);
}
