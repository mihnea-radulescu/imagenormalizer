namespace ImageNormalizer.CommandLine;

public record class CommandLineArguments(
	string InputDirectory, string OutputDirectory, int OutputImageQuality = 80);
