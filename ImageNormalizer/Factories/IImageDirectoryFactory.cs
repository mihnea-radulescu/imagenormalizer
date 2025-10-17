using ImageNormalizer.ImageFileSystem;

namespace ImageNormalizer.Factories;

public interface IImageDirectoryFactory
{
	IImageDirectory Create(Arguments arguments);
}
