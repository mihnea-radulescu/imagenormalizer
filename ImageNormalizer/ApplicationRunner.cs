using System.Text;
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

		var areValidArguments = _argumentsValidator.AreValidArguments(arguments, out string? errorMessage);

		if (areValidArguments)
		{
			var generalInformationText = GetGeneralInformationText(arguments);

			_logger.Info(generalInformationText);
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

	private readonly IArgumentsFactory _argumentsFactory;
	private readonly IArgumentsValidator _argumentsValidator;
	private readonly IImageDirectoryFactory _imageDirectoryFactory;
	private readonly ILogger _logger;

	private static string GetGeneralInformationText(Arguments arguments)
	{
		var generalInformationTextBuilder = new StringBuilder();

		generalInformationTextBuilder.Append($@"Normalizing images from input directory ""{arguments.InputPath}""");
		generalInformationTextBuilder.Append($@" to output directory ""{arguments.OutputPath}""");

		generalInformationTextBuilder.Append(
			$", resizing to output maximum image width/height {arguments.OutputMaximumImageSize}");

		if (arguments.ShouldRemoveImageProfileData)
		{
			generalInformationTextBuilder.Append(", removing image profile data");
		}

		generalInformationTextBuilder.Append($", to output image quality {arguments.OutputImageQuality}");

		generalInformationTextBuilder.Append(
			$", using maximum degree of parallelism {arguments.MaxDegreeOfParallelism}.");

		var generalInformationText = generalInformationTextBuilder.ToString();
		return generalInformationText;
	}
}
