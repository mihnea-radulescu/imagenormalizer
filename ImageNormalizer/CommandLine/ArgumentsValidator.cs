using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageNormalizer.CommandLine;

public class ArgumentsValidator : IArgumentsValidator
{
    static ArgumentsValidator()
	{
		var invalidPathChars = Path.GetInvalidPathChars();
		var additionalInvalidPathChars = new List<char>
		{
			'"',
			'\'',
			'<',
			'>',
			';',
			'*',
			'?'
		};

		var invalidChars = invalidPathChars.Union(additionalInvalidPathChars);

		InvalidChars = new HashSet<char>(invalidChars);
	}
	
	public bool AreValidArguments(Arguments arguments, out string? errorMessage)
	{
		var inputDirectory = arguments.InputPath;
		var outputDirectory = arguments.OutputPath;
		var outputMaximumImageSize = arguments.OutputMaximumImageSize;
		var outputImageQuality = arguments.OutputImageQuality;

		if (!IsDirectoryPathValid(inputDirectory))
		{
			errorMessage = $@"The directory path ""{inputDirectory}"" is invalid.";
			return false;
		}

		if (!IsDirectoryPathValid(outputDirectory))
		{
			errorMessage = $@"The directory path ""{outputDirectory}"" is invalid.";
			return false;
		}

		if (!ExistsDirectory(inputDirectory))
		{
			errorMessage = $@"The input directory ""{inputDirectory}"" does not exist.";
			return false;
		}

		if (AreIdenticalInputDirectoryAndOutputDirectory(inputDirectory, outputDirectory))
		{
			errorMessage = $@"The input directory ""{inputDirectory}"" and output directory ""{outputDirectory}"" are identical.";
			return false;
		}

		if (AreInParentChildRelationship(inputDirectory, outputDirectory))
		{
			errorMessage = $@"The input directory ""{inputDirectory}"" and output directory ""{outputDirectory}"" are in a parent-child relationship.";
			return false;
		}

		if (!HasExpectedOutputMaximumImageSize(outputMaximumImageSize))
		{
			errorMessage = "The output maximum image size is outside of the expected range.";
			return false;
		}

		if (!HasExpectedOutputImageQuality(outputImageQuality))
		{
			errorMessage = "The output image quality is outside of the expected range.";
			return false;
		}

		errorMessage = null;
		return true;
	}

	#region Private

	private static HashSet<char> InvalidChars;

	private static bool IsDirectoryPathValid(string directoryPath)
	{
		for (var i = 0; i < directoryPath.Length; i++)
		{
			if (InvalidChars.Contains(directoryPath[i]))
			{
				return false;
			}
		}

		return true;
	}

	private static bool ExistsDirectory(string directory)
		=> Directory.Exists(directory);

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

	private static bool HasExpectedOutputMaximumImageSize(int outputMaximumImageSize)
		=> outputMaximumImageSize is >= 10 and <= 10000;

	private static bool HasExpectedOutputImageQuality(int outputImageQuality)
		=> outputImageQuality is >= 1 and <= 100;

	#endregion
}
