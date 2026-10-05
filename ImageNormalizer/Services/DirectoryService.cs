using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageNormalizer.Services;

public class DirectoryService : IDirectoryService
{
	public IReadOnlyList<string> GetFileNames(string directoryPath)
	{
		var directoryInfo = new DirectoryInfo(directoryPath);

		var fileNames = directoryInfo
			.GetFiles()
			.Select(aFile => aFile.Name)
			.OrderBy(aFileName => aFileName)
			.ToList();

		return fileNames;
	}

	public IReadOnlyList<string> GetSubDirectoryNames(string directoryPath)
	{
		var directoryInfo = new DirectoryInfo(directoryPath);

		var subDirectoryNames = directoryInfo
			.GetDirectories()
			.Select(aSubDirectory => aSubDirectory.Name)
			.OrderBy(aSubDirectoryName => aSubDirectoryName)
			.ToList();

		return subDirectoryNames;
	}

	public void CreateDirectory(string directoryPath)
		=> Directory.CreateDirectory(directoryPath);
}
