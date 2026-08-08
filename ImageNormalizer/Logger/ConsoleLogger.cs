using System;
using System.Threading;

namespace ImageNormalizer.Logger;

public class ConsoleLogger : ILogger
{
	public ConsoleLogger()
	{
		_consoleLock = new Lock();
	}

	public void NewLine() => Console.Out.WriteLine();

	public void Info(string message) => Console.Out.WriteLine(message);

	public void Error(string message)
	{
		lock (_consoleLock)
		{
			var consoleForegroundColor = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(message);

			Console.ForegroundColor = consoleForegroundColor;
		}
	}

	public void Error(Exception ex) => Error(ex.Message);

	private readonly Lock _consoleLock;
}
