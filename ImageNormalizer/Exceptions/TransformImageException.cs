using System;

namespace ImageNormalizer.Exceptions;

public class TransformImageException : Exception
{
	public TransformImageException() : base()
	{
	}

	public TransformImageException(string? message) : base(message)
	{
	}

	public TransformImageException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}
}
