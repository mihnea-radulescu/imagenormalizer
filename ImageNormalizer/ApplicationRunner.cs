using System;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.Logger;

namespace ImageNormalizer;

public class ApplicationRunner : IApplicationRunner
{
	public ApplicationRunner(
		IArgumentsFactory argumentsFactory,
		IArgumentsValidator argumentsValidator,
		IImageDirectoryInfoFactory imageDirectoryInfoFactory,
		ILogger logger)
	{
		_argumentsFactory = argumentsFactory;
		_argumentsValidator = argumentsValidator;
		_imageDirectoryInfoFactory = imageDirectoryInfoFactory;
		_logger = logger;
	}

	public void Run(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality,
		int maxDegreeOfParallelism)
	{
		var arguments = _argumentsFactory.Create(
			inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality, maxDegreeOfParallelism);

		var areValidArguments = _argumentsValidator.AreValidArguments(arguments, out string? errorMessage);

		if (!areValidArguments)
		{
			_logger.Error(errorMessage!);

			return;
		}

		_logger.Info(
			$@"Normalizing images from input directory ""{arguments.InputPath}"" to output directory ""{arguments.OutputPath}"", resizing to output maximum image width/height {arguments.OutputMaximumImageSize}, at output image quality {arguments.OutputImageQuality}, using maximum degree of parallelism {arguments.MaxDegreeOfParallelism}.");
		_logger.NewLine();

		try
		{
			var imageDirectoryInfo = _imageDirectoryInfoFactory.Create(arguments);

			imageDirectoryInfo.BuildFileSystemInfo();
			imageDirectoryInfo.NormalizeFileSystemInfo();
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	#region Private

	private readonly IArgumentsFactory _argumentsFactory;
	private readonly IArgumentsValidator _argumentsValidator;
	private readonly IImageDirectoryInfoFactory _imageDirectoryInfoFactory;
	private readonly ILogger _logger;

	#endregion
}
