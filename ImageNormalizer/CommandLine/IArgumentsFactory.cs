namespace ImageNormalizer.CommandLine;

public interface IArgumentsFactory
{
	Arguments Create(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality,
		int maxDegreeOfParallelism);
}
