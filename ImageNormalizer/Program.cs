using Cocona;
using Microsoft.Extensions.DependencyInjection;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;
using ImageNormalizer.Factories;

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
		services.AddSingleton<ITransformImageFactory, ImageSharpTransformImageFactory>();
		services.AddSingleton<IImageNormalizerService, ImageNormalizerService>();
		services.AddSingleton<IImageDirectoryInfoFactory, ImageDirectoryInfoFactory>();
		services.AddSingleton<IApplicationRunner, ApplicationRunner>();
	}

	private static void RegisterCommands(CoconaApp app)
	{
		app.AddCommand(
			(
				[Argument(Description = "The input directory")] string inputDirectory,
				[Argument(Description = "The output directory")] string outputDirectory,
				[Option("quality", ['q'], Description = "The output image quality")] int outputImageQuality = 80
			) =>
			{
				var applicationRunner = app.Services.GetService<IApplicationRunner>();

				applicationRunner!.Run(inputDirectory, outputDirectory, outputImageQuality);
			});
	}

	#endregion
}
