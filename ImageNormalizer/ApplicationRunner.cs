using System;
using System.Collections.Generic;
using ImageNormalizer.CommandLine;
using ImageNormalizer.Factories;
using ImageNormalizer.FileSystemInfo;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer;

public class ApplicationRunner : IApplicationRunner
{
	public ApplicationRunner(IReadOnlyList<string> args)
    {
		_args = args;
	}

    public void Run()
	{
		ILogger logger = new ConsoleLogger();
		
		ICommandLineParser commandLineParser = new CommandLineParser();
		var commandLineArguments = commandLineParser.GetCommandLineArguments(_args);

		if (commandLineArguments is null)
		{
			ICommandLineHelp commandLineHelp = new CommandLineHelp();
			var helpText = commandLineHelp.HelpText;

			logger.Info(helpText);
		}
		else
		{
			var inputDirectory = commandLineArguments.InputDirectory;
			var outputDirectory = commandLineArguments.OutputDirectory;
			var outputImageQuality = commandLineArguments.OutputImageQuality;
			
			IImageFileExtensionService imageFileExtensionService = new ImageFileExtensionService();

			ITransformImageFactory transformImageFactory =
				new ImageSharpTransformImageFactory(outputImageQuality);
			IImageNormalizerService imageNormalizerService =
				new ImageNormalizerService(transformImageFactory);

			try
			{
				var imageDirectoryInfo = new ImageDirectoryInfo(
					imageFileExtensionService,
					imageNormalizerService,
					logger,
					inputDirectory,
					outputDirectory);

				imageDirectoryInfo.BuildFileSystemInfo();
				imageDirectoryInfo.NormalizeFileSystemInfo();
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
		}
	}

	#region Private

	private readonly IReadOnlyList<string> _args;

	#endregion
}
