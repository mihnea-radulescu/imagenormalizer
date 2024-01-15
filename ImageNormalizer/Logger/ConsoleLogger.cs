using System;

namespace ImageNormalizer.Logger;

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
