namespace ImageNormalizer.CommandLine;

public record class Arguments(
	string InputDirectory, string OutputDirectory, int OutputImageQuality);
