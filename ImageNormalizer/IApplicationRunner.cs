namespace ImageNormalizer;

public interface IApplicationRunner
{
	ExitCode Run(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality,
		bool shouldRemoveImageProfileData,
		int maxDegreeOfParallelism);
}
