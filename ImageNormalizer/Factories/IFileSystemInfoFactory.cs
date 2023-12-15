using ImageNormalizer.FileSystemInfo;

namespace ImageNormalizer.Factories;

public interface IFileSystemInfoFactory
{
	IFileSystemInfo GetFileSystemInfo(string inputDirectory, string outputDirectory);
}
