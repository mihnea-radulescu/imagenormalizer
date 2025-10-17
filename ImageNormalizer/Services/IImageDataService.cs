using System.IO;

namespace ImageNormalizer.Services;

public interface IImageDataService
{
	Stream? ReadImageDataFromDisc(Arguments arguments);

	void WriteImageDataToDisc(Stream outputImageDataStream, Arguments arguments);
}
