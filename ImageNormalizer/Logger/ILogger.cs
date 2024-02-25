using System;

namespace ImageNormalizer.Logger;

public interface ILogger
{
	void Info(string message);

	void Error(string message);
	void Error(Exception ex);
}
