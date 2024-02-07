using System.Collections.Generic;

namespace ImageNormalizer.Services;

public interface IDirectoryService
{
	IReadOnlyList<string> GetFiles(string directory);
	IReadOnlyList<string> GetSubDirectories(string directory);

	void CreateDirectory(string directoryPath);
}
