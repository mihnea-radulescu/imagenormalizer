using System;
using ImageNormalizer.Attributes;

namespace ImageNormalizer.Logger;

[StatelessService(Remark =
	"Although not strictly stateless, console logging can be regarded as conceptually stateless, and used as a Singleton dependency.")]
public class ConsoleLogger : ILogger
{
	public void Info(string message)
	{
		Console.Out.WriteLine(message);
	}

	public void Error(Exception ex)
	{
		Console.Error.WriteLine(ex);
	}
}
