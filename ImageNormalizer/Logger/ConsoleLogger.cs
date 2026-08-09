using System;
using System.Threading;

namespace ImageNormalizer.Logger;

public class ConsoleLogger : ILogger
{
	public ConsoleLogger()
	{
		_consoleForegroundColor = Console.ForegroundColor;

		_consoleLock = new Lock();
	}

	public void NewLine()
	{
		lock (_consoleLock)
		{
			Console.Out.WriteLine();
		}
	}

	public void Info(string message)
	{
		lock (_consoleLock)
		{
			Console.Out.WriteLine(message);
		}
	}

	public void Error(string message)
	{
		lock (_consoleLock)
		{
			Console.ForegroundColor = ConsoleColor.Red;

			Console.Error.WriteLine(message);

			Console.ForegroundColor = _consoleForegroundColor;
		}
	}

	public void Error(Exception ex) => Error(ex.Message);

	private readonly ConsoleColor _consoleForegroundColor;

	private readonly Lock _consoleLock;
}
