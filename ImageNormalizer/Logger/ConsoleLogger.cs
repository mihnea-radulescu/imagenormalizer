using System;

namespace ImageNormalizer.Logger;

public class ConsoleLogger : ILogger
{
	public void NewLine() => Console.Out.WriteLine();
	
	public void Info(string message) => Console.Out.WriteLine(message);
	
	public void Error(string message) => Console.Error.WriteLine($"Error: {message}");

	public void Error(Exception ex) => Console.Error.WriteLine($"Error: {ex}");
}
