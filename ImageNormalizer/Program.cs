using Cocona;
using Microsoft.Extensions.DependencyInjection;
using ImageNormalizer.Adapters;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer;

public class Program
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
		services.AddSingleton<IImageTransformer, ImageSharpImageTransformer>();
		services.AddSingleton<IImageNormalizerService, ImageNormalizerService>();
		services.AddSingleton<IDirectoryService, DirectoryService>();
		services.AddSingleton<IImageDirectoryInfoFactory, ImageDirectoryInfoFactory>();
		services.AddSingleton<IApplicationRunner, ApplicationRunner>();
	}

	private static void RegisterCommands(CoconaApp app)
	{
		app.AddCommand(
			(
				[Argument(Description = "The input directory")] string inputDirectory,
				[Argument(Description = "The output directory")] string outputDirectory,
				[Option("max-width-height", ['m'], Description = "The output maximum image width/height")] int outputMaximumImageSize = 3840,
				[Option("quality", ['q'], Description = "The output image quality")] int outputImageQuality = 80
			) =>
			{
				var applicationRunner = app.Services.GetService<IApplicationRunner>();

				applicationRunner!.Run(
					inputDirectory, outputDirectory, outputMaximumImageSize, outputImageQuality);
			});
	}

	#endregion
}
