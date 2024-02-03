namespace ImageNormalizer.CommandLine;

public interface IArgumentsValidator
{
	bool AreValidArguments(Arguments arguments, out string? errorMessage);
}
