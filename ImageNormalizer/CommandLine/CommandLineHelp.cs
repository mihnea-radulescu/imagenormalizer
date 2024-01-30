using System.Text;

namespace ImageNormalizer.CommandLine;

public class CommandLineHelp : ICommandLineHelp
{
    public CommandLineHelp()
    {
        var helpTextBuilder = new StringBuilder();

		helpTextBuilder.AppendLine();

		helpTextBuilder.AppendLine("Usage: ImageNormalizer [InputDirectory] [OutputDirectory] [OutputImageQuality]");
        helpTextBuilder.AppendLine();

		helpTextBuilder.AppendLine("InputDirectory:");
		helpTextBuilder.AppendLine("\tRequired - The input directory containing the images to be processed (recursive processing).");
        helpTextBuilder.AppendLine();

		helpTextBuilder.AppendLine("OutputDirectory:");
		helpTextBuilder.AppendLine("\tRequired - The output directory to place the processed images into.");
		helpTextBuilder.AppendLine();

		helpTextBuilder.AppendLine("OutputImageQuality:");
        helpTextBuilder.Append("\tOptional - The output image quality, as an integer between 1 and 100 (default is 80).");

		_helpText = helpTextBuilder.ToString();
    }

    public string HelpText => _helpText;

	#region Private

    private readonly string _helpText;

	#endregion
}
