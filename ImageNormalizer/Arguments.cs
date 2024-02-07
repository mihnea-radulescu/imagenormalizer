namespace ImageNormalizer;

public record class Arguments(
	string InputPath, string OutputPath, int OutputMaximumImageSize, int OutputImageQuality);
