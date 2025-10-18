using System.ComponentModel.DataAnnotations;
using Cocona;
using Microsoft.Extensions.DependencyInjection;
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
		var builder = CoconaApp.CreateBuilder(args);
		RegisterDependencies(builder.Services);

		var app = builder.Build();
		RegisterCommands(app);

		app.Run();
	}

	#region Private

	private static void RegisterDependencies(IServiceCollection services)
	{
		services.AddSingleton<IArgumentsFactory, ArgumentsFactory>();
		services.AddSingleton<ILogger, ConsoleLogger>();
		services.AddSingleton<IArgumentsValidator, ArgumentsValidator>();
		services.AddSingleton<IImageFileExtensionService, ImageFileExtensionService>();
		services.AddSingleton<IImageResizeCalculator, ImageResizeCalculator>();
		services.AddSingleton<IImageTransformer, ImageTransformer>();
		services.AddSingleton<IImageDataService, ImageDataService>();
		services.AddSingleton<IImageNormalizerService, ImageNormalizerService>();
		services.AddSingleton<IDirectoryService, DirectoryService>();
		services.AddSingleton<IImageDirectoryFactory, ImageDirectoryFactory>();
		services.AddSingleton<IApplicationRunner, ApplicationRunner>();
	}

	private static void RegisterCommands(CoconaApp app)
	{
		app.AddCommand(
			(
				[Argument(Description = "The input directory")]
				string inputDirectory,

				[Argument(Description = "The output directory, to be created, if it does not exist")]
				string outputDirectory,

				[Option("max-width-height", ['m'], Description = "The output maximum image width/height")]
				[Range(1, 15360)]
				int outputMaximumImageSize = 3840,

				[Option("quality", ['q'], Description = "The output image quality")]
				[Range(1, 100)]
				int outputImageQuality = 80,

				[Option("remove-profile-data", ['r'], Description = "Removes image profile data")]
				bool shouldRemoveImageProfileData = false,

				[Option("max-degree-of-parallelism", ['p'], Description = "The maximum degree of parallel image processing, upper-bounded by processor count")]
				[Range(1, 128)]
				int maxDegreeOfParallelism = 4
			) =>
			{
				var applicationRunner = app.Services.GetService<IApplicationRunner>();

				applicationRunner!.Run(
					inputDirectory,
					outputDirectory,
					outputMaximumImageSize,
					outputImageQuality,
					shouldRemoveImageProfileData,
					maxDegreeOfParallelism);
			});
	}

	#endregion
}
