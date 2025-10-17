using System.IO;

namespace ImageNormalizer.Services;

public interface IImageNormalizerService
{
	Stream? NormalizeImage(Stream inputImageDataStream, Arguments arguments);
}
