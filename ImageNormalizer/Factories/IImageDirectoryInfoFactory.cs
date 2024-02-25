using ImageNormalizer.FileSystemInfo;

namespace ImageNormalizer.Factories;

public interface IImageDirectoryInfoFactory
{
	ImageDirectoryInfo Create(Arguments arguments);
}
