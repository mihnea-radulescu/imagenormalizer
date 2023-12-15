using System;
using System.Collections.Generic;
using System.IO;
using ImageNormalizer.Attributes;

namespace ImageNormalizer.CommandLine;

[StatelessService]
public class CommandLineParser : ICommandLineParser
{
    public CommandLineArguments? GetCommandLineArguments(IReadOnlyList<string> args)
	{
		if (!HasExpectedArgumentCount(args.Count))
		{
			return null;
		}

		var inputDirectory = args[0];
		var outputDirectory = args[1];

		if (AreIdenticalInputDirectoryAndOutputDirectory(inputDirectory, outputDirectory))
		{
			return null;
		}

		if (!ExistsInputDirectory(inputDirectory))
		{
			return null;
		}

		if (args.Count == 2)
		{
			return new CommandLineArguments(inputDirectory, outputDirectory);
		}

		if (args.Count == 3)
		{
			var outputImageQualityText = args[2];
			int outputImageQuality;

			if (HasExpectedOutputImageQuality(outputImageQualityText, out outputImageQuality))
			{
				return new CommandLineArguments(inputDirectory, outputDirectory, outputImageQuality);
			}
		}

		return null;
	}

	private static bool HasExpectedOutputImageQuality(
		string outputImageQualityText, out int outputImageQuality)
			=> int.TryParse(outputImageQualityText, out outputImageQuality) &&
			   outputImageQuality >= 1 &&
			   outputImageQuality <= 100;

	#region Private

	private bool HasExpectedArgumentCount(int argumentCount)
		=> argumentCount == 2 || argumentCount == 3;

	private bool AreIdenticalInputDirectoryAndOutputDirectory(
		string inputDirectory, string outputDirectory)
			=> inputDirectory.Equals(outputDirectory, StringComparison.InvariantCultureIgnoreCase);

	private bool ExistsInputDirectory(string inputDirectory) => Directory.Exists(inputDirectory);

	#endregion
}
