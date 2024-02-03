using ImageNormalizer.FileSystemInfo;

namespace ImageNormalizer.Factories;

public interface IImageDirectoryInfoFactory
{
	ImageDirectoryInfo Create(
		string inputDirectory, string outputDirectory, int outputImageQuality);
}
