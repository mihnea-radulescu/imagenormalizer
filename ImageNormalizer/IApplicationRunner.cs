namespace ImageNormalizer;

public interface IApplicationRunner
{
	void Run(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality);
}
