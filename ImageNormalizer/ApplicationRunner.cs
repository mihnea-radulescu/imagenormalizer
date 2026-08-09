using System.Text;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer;

public class ApplicationRunner : IApplicationRunner
{
	public ApplicationRunner(
		IArgumentsFactory argumentsFactory,
		IArgumentsValidator argumentsValidator,
		IDirectoryService directoryService,
		IImageDirectoryFactory imageDirectoryFactory,
		ILogger logger)
	{
		_argumentsFactory = argumentsFactory;
		_argumentsValidator = argumentsValidator;
		_directoryService = directoryService;
		_imageDirectoryFactory = imageDirectoryFactory;
		_logger = logger;
	}

	public ExitCode Run(
		string inputDirectory,
		string outputDirectory,
		int outputMaximumImageSize,
		int outputImageQuality,
		bool shouldRemoveImageProfileData,
		int maxDegreeOfParallelism)
	{
		ExitCode exitCode;

		var arguments = _argumentsFactory.Create(
			inputDirectory,
			outputDirectory,
			outputMaximumImageSize,
			outputImageQuality,
			shouldRemoveImageProfileData,
			maxDegreeOfParallelism);

		var areValidArguments = _argumentsValidator.AreValidArguments(
			arguments, out string? invalidArgumentsErrorMessage);

		if (areValidArguments)
		{
			var generalInformationText = GetGeneralInformationText(arguments);

			_logger.Info(generalInformationText);
			_logger.NewLine();

			var imageDirectory = _imageDirectoryFactory.Create(arguments);
			_directoryService.CreateDirectory(arguments.OutputPath);

			exitCode = imageDirectory.BuildImageDirectory();

			if (exitCode == ExitCode.Successful)
			{
				exitCode = imageDirectory.NormalizeImages();
			}
		}
		else
		{
			exitCode = ExitCode.InvalidArguments;
		}

		LogRunInformation(exitCode, invalidArgumentsErrorMessage);

		return exitCode;
	}

	private readonly IArgumentsFactory _argumentsFactory;
	private readonly IArgumentsValidator _argumentsValidator;
	private readonly IDirectoryService _directoryService;
	private readonly IImageDirectoryFactory _imageDirectoryFactory;
	private readonly ILogger _logger;

	private static string GetGeneralInformationText(Arguments arguments)
	{
		var generalInformationTextBuilder = new StringBuilder();

		generalInformationTextBuilder.Append(
			$@"Normalizing images from input directory ""{arguments.InputPath}""");
		generalInformationTextBuilder.Append(
			$@" to output directory ""{arguments.OutputPath}""");

		generalInformationTextBuilder.Append(
			$", resizing to output maximum image width/height {arguments.OutputMaximumImageSize}");

		if (arguments.ShouldRemoveImageProfileData)
		{
			generalInformationTextBuilder.Append(
				", removing image profile data");
		}

		generalInformationTextBuilder.Append(
			$", to output image quality {arguments.OutputImageQuality}");

		generalInformationTextBuilder.Append(
			$", using maximum degree of parallelism {arguments.MaxDegreeOfParallelism}.");

		var generalInformationText = generalInformationTextBuilder.ToString();
		return generalInformationText;
	}

	private void LogRunInformation(
		ExitCode exitCode, string? invalidArgumentsErrorMessage)
	{
		switch (exitCode)
		{
			case ExitCode.Successful:
				_logger.NewLine();
				_logger.Info("Execution successful.");
				break;

			case ExitCode.InvalidArguments:
				_logger.Error(invalidArgumentsErrorMessage!);
				break;

			case ExitCode.Aborted:
				_logger.NewLine();
				_logger.Error("Execution aborted.");
				break;
		}
	}
}
