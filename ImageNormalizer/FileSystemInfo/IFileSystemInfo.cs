namespace ImageNormalizer.FileSystemInfo;

public interface IFileSystemInfo
{
	string InputPath { get; }
	string OutputPath { get; }

	void BuildFileSystemInfo();
	void NormalizeFileSystemInfo();
}
