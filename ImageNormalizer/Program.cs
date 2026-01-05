using System.CommandLine;
using ImageNormalizer.Adapters;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer;

public static class Program
{
	public static void Main(string[] args)
	{
		var inputDirectoryArgument = new Argument<string>("inputDirectory")
		{
			Description = "The input directory"
		};
		var outputDirectoryArgument = new Argument<string>("outputDirectory")
		{
			Description = "The output directory, to be created, if it does not exist"
		};

		var outputMaximumImageSizeOption = new Option<int>("--max-width-height", "-m")
		{
			Description = "The output maximum image width/height",
			DefaultValueFactory = _ => 3840
		};
		outputMaximumImageSizeOption.Validators.Add(result =>
		{
			var value = result.GetValueOrDefault<int>();

			if (value < 10 || value > 15360)
			{
				result.AddError("max-width-height must be between 10 and 15360.");
			}
		});

		var outputImageQualityOption = new Option<int>("--quality", "-q")
		{
			Description = "The output image quality",
			DefaultValueFactory = _ => 80
		};
		outputImageQualityOption.Validators.Add(result =>
		{
			var value = result.GetValueOrDefault<int>();

			if (value is < 10 or > 100)
			{
				result.AddError("quality must be between 10 and 100.");
			}
		});

		var shouldRemoveImageProfileDataOption = new Option<bool>("--remove-profile-data", "-r")
		{
			Description = "Removes image profile data",
			DefaultValueFactory = _ => false
		};

		var maxDegreeOfParallelismOption = new Option<int>("--max-degree-of-parallelism", "-p")
		{
			Description = "The maximum degree of parallel image processing, upper-bounded by processor count",
			DefaultValueFactory = _ => 4
		};
		maxDegreeOfParallelismOption.Validators.Add(result =>
		{
			var value = result.GetValueOrDefault<int>();

			if (value is < 1 or > 128)
			{
				result.AddError("max-degree-of-parallelism must be between 1 and 128.");
			}
		});

		var rootCommand = new RootCommand(
			"Image Normalizer - batch-processing tool that resizes and compresses images")
		{
			inputDirectoryArgument,
			outputDirectoryArgument,
			outputMaximumImageSizeOption,
			outputImageQualityOption,
			shouldRemoveImageProfileDataOption,
			maxDegreeOfParallelismOption
		};

		rootCommand.SetAction(result =>
		{
			var inputDirectory = result.GetValue(inputDirectoryArgument)!;
			var outputDirectory = result.GetValue(outputDirectoryArgument)!;

			var outputMaximumImageSize = result.GetValue(outputMaximumImageSizeOption);
			var outputImageQuality = result.GetValue(outputImageQualityOption);
			var shouldRemoveImageProfileData = result.GetValue(shouldRemoveImageProfileDataOption);
			var maxDegreeOfParallelism = result.GetValue(maxDegreeOfParallelismOption);

			var applicationRunner = BuildApplicationRunner();

			applicationRunner.Run(
				inputDirectory,
				outputDirectory,
				outputMaximumImageSize,
				outputImageQuality,
				shouldRemoveImageProfileData,
				maxDegreeOfParallelism);
		});

		rootCommand.Parse(args).Invoke();
	}

	private static IApplicationRunner BuildApplicationRunner()
	{
		IArgumentsFactory argumentsFactory = new ArgumentsFactory();
		IArgumentsValidator argumentsValidator = new ArgumentsValidator();

		ILogger logger = new ConsoleLogger();

		IImageFileExtensionService imageFileExtensionService = new ImageFileExtensionService();
		IImageDataService imageDataService = new ImageDataService(logger);

		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageTransformer imageTransformer = new ImageTransformer(imageResizeCalculator);
		IImageNormalizerService imageNormalizerService = new ImageNormalizerService(imageTransformer, logger);

		IDirectoryService directoryService = new DirectoryService();

		IImageDirectoryFactory imageDirectoryFactory = new ImageDirectoryFactory(
			imageFileExtensionService,
			imageDataService,
			imageNormalizerService,
			directoryService,
			logger);

		IApplicationRunner applicationRunner = new ApplicationRunner(
			argumentsFactory, argumentsValidator, imageDirectoryFactory, logger);

		return applicationRunner;
	}
}
