using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageNormalizer.Services;

public class DirectoryService : IDirectoryService
{
    public IReadOnlyList<string> GetFiles(string directory)
	{
		var directoryInfo = new DirectoryInfo(directory);

		var files = directoryInfo
			.GetFiles()
			.Select(aFile => aFile.Name)
			.OrderBy(aFileName => aFileName)
			.ToList();

		return files;
	}

	public IReadOnlyList<string> GetSubDirectories(string directory)
	{
		var directoryInfo = new DirectoryInfo(directory);

		var subDirectories = directoryInfo
			.GetDirectories()
			.Select(aSubDirectory => aSubDirectory.Name)
			.OrderBy(aSubDirectoryName => aSubDirectoryName)
			.ToList();

		return subDirectories;
	}

	public void CreateDirectory(string directoryPath)
	{
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}
	}
}
