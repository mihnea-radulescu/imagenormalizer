namespace ImageNormalizer;

public record Arguments(
	string InputPath,
	string OutputPath,
	int OutputMaximumImageSize,
	int OutputImageQuality,
	int MaxDegreeOfParallelism);
