using System.IO;

namespace ImageNormalizer.Adapters;

public interface IImageTransformer
{
	Stream TransformImage(Stream inputImageDataStream, Arguments arguments);
}
