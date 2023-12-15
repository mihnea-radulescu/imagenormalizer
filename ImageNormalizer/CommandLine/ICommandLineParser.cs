using System.Collections.Generic;

namespace ImageNormalizer.CommandLine;

public interface ICommandLineParser
{
	CommandLineArguments? GetCommandLineArguments(IReadOnlyList<string> args);
}
