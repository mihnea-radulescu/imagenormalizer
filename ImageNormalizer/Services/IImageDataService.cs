using System.IO;

namespace ImageNormalizer.Services;

public interface IImageDataService
{
	bool ExistsOutputImageDataOnDisc(Arguments arguments);

	Stream? ReadImageDataFromDisc(Arguments arguments);

	void WriteImageDataToDisc(
		Stream outputImageDataStream, Arguments arguments);
}
