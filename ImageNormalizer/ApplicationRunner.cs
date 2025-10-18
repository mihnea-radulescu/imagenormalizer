using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.Logger;

namespace ImageNormalizer;

public class ApplicationRunner : IApplicationRunner
{
	public ApplicationRunner(
		IArgumentsFactory argumentsFactory,
		IArgumentsValidator argumentsValidator,
		IImageDirectoryFactory imageDirectoryFactory,
		ILogger logger)
	{
		_argumentsFactory = argumentsFactory;
		_argumentsValidator = argumentsValidator;
		_imageDirectoryFactory = imageDirectoryFactory;
		_logger = logger;
	}

	public void Run(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality,
		bool shouldRemoveImageProfileData,
		int maxDegreeOfParallelism)
	{
		var arguments = _argumentsFactory.Create(
			inputDirectory,
			outputDirectory,
			outputMaximumImageSize,
			outputImageQuality,
			shouldRemoveImageProfileData,
			maxDegreeOfParallelism);

		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? errorMessage);

		if (areValidArguments)
		{
			_logger.Info(
				$@"Normalizing images from input directory ""{arguments.InputPath}"" to output directory ""{arguments.OutputPath}"", resizing to output maximum image width/height {arguments.OutputMaximumImageSize}, at output image quality {arguments.OutputImageQuality}, using maximum degree of parallelism {arguments.MaxDegreeOfParallelism}.");
			_logger.NewLine();

			var imageDirectory = _imageDirectoryFactory.Create(arguments);

			imageDirectory.BuildImageDirectory();
			imageDirectory.NormalizeImages();
		}
		else
		{
			_logger.Error(errorMessage!);
		}
	}

	#region Private

	private readonly IArgumentsFactory _argumentsFactory;
	private readonly IArgumentsValidator _argumentsValidator;
	private readonly IImageDirectoryFactory _imageDirectoryFactory;
	private readonly ILogger _logger;

	#endregion
}
