using System;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageNormalizer.Adapters;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer;

public static class Program
{
	public static int Main(string[] args)
	{
		Console.TreatControlCAsInput = true;

		var exitCode = ExitCode.Successful;
		var cancellationTokenSource = new CancellationTokenSource();

		var applicationRunnerTaskPollingInterval =
			TimeSpan.FromMilliseconds(250);

		var inputDirectoryArgument = new Argument<string>("inputDirectory")
		{
			Description = "The input directory"
		};
		var outputDirectoryArgument = new Argument<string>("outputDirectory")
		{
			Description =
				"The output directory, to be created, if it does not exist"
		};

		var outputMaximumImageSizeOption = new Option<int>(
			"--max-width-height", "-m")
		{
			Description = "The output image maximum width or height, whichever is higher",
			DefaultValueFactory = _ => 3840
		};
		outputMaximumImageSizeOption.Validators.Add(result =>
		{
			var value = result.GetValueOrDefault<int>();

			if (value is < 10 or > 15360)
			{
				result.AddError(
					"max-width-height must be between 10 and 15360.");
				exitCode = ExitCode.InvalidArguments;
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
				exitCode = ExitCode.InvalidArguments;
			}
		});

		var shouldRemoveImageProfileDataOption = new Option<bool>(
			"--remove-profile-data", "-r")
		{
			Description = "Removes image profile data",
			DefaultValueFactory = _ => false
		};

		var maxDegreeOfParallelismOption = new Option<int>(
			"--max-degree-of-parallelism", "-p")
		{
			Description =
				"The maximum degree of parallel image processing, upper-bounded by processor count",
			DefaultValueFactory = _ => 4
		};
		maxDegreeOfParallelismOption.Validators.Add(result =>
		{
			var value = result.GetValueOrDefault<int>();

			if (value is < 1 or > 128)
			{
				result.AddError(
					"max-degree-of-parallelism must be between 1 and 128.");
				exitCode = ExitCode.InvalidArguments;
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

		rootCommand.SetAction(parseResult =>
		{
			var inputDirectory = parseResult.GetValue(inputDirectoryArgument)!;
			var outputDirectory = parseResult.GetValue(
				outputDirectoryArgument)!;

			var outputMaximumImageSize = parseResult.GetValue(
				outputMaximumImageSizeOption);
			var outputImageQuality = parseResult.GetValue(
				outputImageQualityOption);
			var shouldRemoveImageProfileData = parseResult.GetValue(
				shouldRemoveImageProfileDataOption);
			var maxDegreeOfParallelism = parseResult.GetValue(
				maxDegreeOfParallelismOption);

			var applicationRunner = BuildApplicationRunner(
				cancellationTokenSource);

			var applicationRunnerTask = Task.Run(() =>
				{
					exitCode = applicationRunner.Run(
						inputDirectory,
						outputDirectory,
						outputMaximumImageSize,
						outputImageQuality,
						shouldRemoveImageProfileData,
						maxDegreeOfParallelism);
				});

			HandleApplicationRunnerTaskExecution(
				applicationRunnerTask,
				cancellationTokenSource,
				applicationRunnerTaskPollingInterval);
		});

		var rootCommandParseResult = rootCommand.Parse(args);
		if (rootCommandParseResult.Errors.Any())
		{
			exitCode = ExitCode.InvalidArguments;
		}
		rootCommandParseResult.Invoke();

		return (int)exitCode;
	}

	private static IApplicationRunner BuildApplicationRunner(
		CancellationTokenSource cancellationTokenSource)
	{
		IArgumentsFactory argumentsFactory = new ArgumentsFactory();
		IArgumentsValidator argumentsValidator = new ArgumentsValidator();

		ILogger logger = new ConsoleLogger();

		IImageFileExtensionService imageFileExtensionService =
			new ImageFileExtensionService();
		IImageDataService imageDataService = new ImageDataService(logger);

		IImageResizeCalculator imageResizeCalculator =
			new ImageResizeCalculator();
		IImageTransformer imageTransformer =
			new ImageTransformer(imageResizeCalculator);
		IImageNormalizerService imageNormalizerService =
			new ImageNormalizerService(imageTransformer, logger);

		IDirectoryService directoryService = new DirectoryService();

		IImageDirectoryFactory imageDirectoryFactory =
			new ImageDirectoryFactory(
				imageFileExtensionService,
				imageDataService,
				imageNormalizerService,
				directoryService,
				logger,
				cancellationTokenSource);

		IApplicationRunner applicationRunner = new ApplicationRunner(
			argumentsFactory,
			argumentsValidator,
			directoryService,
			imageDirectoryFactory,
			logger);

		return applicationRunner;
	}

	private static void HandleApplicationRunnerTaskExecution(
		Task applicationRunnerTask,
		CancellationTokenSource cancellationTokenSource,
		TimeSpan applicationRunnerTaskPollingInterval)
	{
		do
		{
			if (!cancellationTokenSource.IsCancellationRequested &&
			    Console.KeyAvailable)
			{
				var keyPressed = Console.ReadKey();
				if (keyPressed is
				    {
					    Modifiers: ConsoleModifiers.Control,
					    Key: ConsoleKey.C
				    })
				{
					cancellationTokenSource.Cancel();
				}
			}

			Thread.Sleep(applicationRunnerTaskPollingInterval);
		} while (!applicationRunnerTask.IsCompleted);
	}
}
