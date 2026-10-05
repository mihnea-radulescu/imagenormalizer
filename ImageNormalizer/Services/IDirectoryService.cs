using System.Collections.Generic;

namespace ImageNormalizer.Services;

public interface IDirectoryService
{
	IReadOnlyList<string> GetFileNames(string directoryPath);
	IReadOnlyList<string> GetSubDirectoryNames(string directoryPath);

	void CreateDirectory(string directoryPath);
}
