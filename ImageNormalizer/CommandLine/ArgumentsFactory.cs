using System.IO;

namespace ImageNormalizer.CommandLine;

public class ArgumentsFactory : IArgumentsFactory
{
	public Arguments Create(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality)
	{
		var inputDirectoryFullPath = Path.GetFullPath(inputDirectory);
		var outputDirectoryFullPath = Path.GetFullPath(outputDirectory);

		var arguments = new Arguments(
			inputDirectoryFullPath,
			outputDirectoryFullPath,
			outputMaximumImageSize,
			outputImageQuality);

		return arguments;
	}
}
