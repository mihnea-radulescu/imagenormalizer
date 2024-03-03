using System;
using System.IO;

namespace ImageNormalizer.CommandLine;

public class ArgumentsFactory : IArgumentsFactory
{
	public Arguments Create(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality,
		int maxDegreeOfParallelism)
	{
		var inputDirectoryFullPath = Path.GetFullPath(inputDirectory);
		var outputDirectoryFullPath = Path.GetFullPath(outputDirectory);

		var processorCount = Environment.ProcessorCount;
		if (maxDegreeOfParallelism > processorCount)
		{
			maxDegreeOfParallelism = processorCount;
		}

		var arguments = new Arguments(
			inputDirectoryFullPath,
			outputDirectoryFullPath,
			outputMaximumImageSize,
			outputImageQuality,
			maxDegreeOfParallelism);

		return arguments;
	}
}
