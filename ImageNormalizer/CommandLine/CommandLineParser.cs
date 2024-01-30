using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageNormalizer.CommandLine;

public class CommandLineParser : ICommandLineParser
{
    public CommandLineArguments? GetCommandLineArguments(IReadOnlyList<string> args)
	{
		if (!HasExpectedArgumentCount(args.Count))
		{
			return null;
		}

		string inputDirectory;
		string outputDirectory;

		try
		{
			inputDirectory = Path.GetFullPath(args[0]);
			outputDirectory = Path.GetFullPath(args[1]);
		}
		catch
		{
			return null;
		}

		if (!ExistsInputDirectory(inputDirectory))
		{
			return null;
		}

		if (AreIdenticalInputDirectoryAndOutputDirectory(inputDirectory, outputDirectory))
		{
			return null;
		}

		if (AreInParentChildRelationship(inputDirectory, outputDirectory))
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

	#region Private

	private static bool HasExpectedArgumentCount(int argumentCount)
		=> argumentCount == 2 || argumentCount == 3;

	private static bool ExistsInputDirectory(string inputDirectory)
		=> Directory.Exists(inputDirectory);

	private static bool AreIdenticalInputDirectoryAndOutputDirectory(
		string inputDirectory, string outputDirectory)
			=> inputDirectory.Equals(outputDirectory, StringComparison.InvariantCultureIgnoreCase);

	private static bool AreInParentChildRelationship(
		string inputDirectory, string outputDirectory)
	{
		var inputDirectoryLower = inputDirectory.ToLowerInvariant();
		var outputDirectoryLower = outputDirectory.ToLowerInvariant();

		if (outputDirectoryLower.StartsWith(inputDirectoryLower) ||
			inputDirectoryLower.StartsWith(outputDirectoryLower))
		{
			var orderedDirectories = new List<string>
			{
				inputDirectoryLower,
				outputDirectoryLower
			}
			.OrderByDescending(aDirectory => aDirectory)
			.ToList();

			var directoryPathDifference = orderedDirectories[0]
				.Substring(orderedDirectories[1].Length);

			var areInParentChildRelationship =
				directoryPathDifference.StartsWith(Path.DirectorySeparatorChar) ||
				directoryPathDifference.StartsWith(Path.AltDirectorySeparatorChar);

			return areInParentChildRelationship;
		}
		else
		{
			return false;
		}
	}

	private static bool HasExpectedOutputImageQuality(
		string outputImageQualityText, out int outputImageQuality)
			=> int.TryParse(outputImageQualityText, out outputImageQuality) &&
			   outputImageQuality >= 1 &&
			   outputImageQuality <= 100;

	#endregion
}
