using System;

namespace ImageNormalizer.Adapters;

public interface ITransformImage : IDisposable
{
	string InputFileName { get; }
	string OutputFileName { get; }
	int OutputImageQuality { get; }

	void SaveTransformedImage();
}
