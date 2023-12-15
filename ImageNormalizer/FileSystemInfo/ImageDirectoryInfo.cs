using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.FileSystemInfo;

public class ImageDirectoryInfo : FileSystemInfoBase
{
	public ImageDirectoryInfo(
		IImageFileExtensionService imageFileExtensionService,
		IImageNormalizerService imageNormalizerService,
		ILogger logger,
		string inputPath,
		string outputPath)
		: base(logger, inputPath, outputPath)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;

		_fileSystemInfoCollection = new List<IFileSystemInfo>();
	}

	#region Protected

	protected override void BuildFileSystemInfoSpecific()
	{
		if (!Directory.Exists(OutputPath))
		{
			Directory.CreateDirectory(OutputPath);
		}

		var directoryInfo = new DirectoryInfo(InputPath);

		AddSubDirectories(directoryInfo);
		AddFiles(directoryInfo);

		foreach (var aFileSystemInfo in _fileSystemInfoCollection)
		{
			aFileSystemInfo.BuildFileSystemInfo();
		}
	}

	protected override void NormalizeFileSystemInfoSpecific()
	{
		_logger.Info(
			$@"Processing input directory ""{InputPath}"" into output directory ""{OutputPath}"".");
		
		foreach (var aFileSystemInfo in _fileSystemInfoCollection)
		{
			aFileSystemInfo.NormalizeFileSystemInfo();
		}
	}

	#endregion

	#region Private

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;

	private readonly List<IFileSystemInfo> _fileSystemInfoCollection;

	private void AddSubDirectories(DirectoryInfo directoryInfo)
	{
		var subDirectories = directoryInfo
			.GetDirectories()
			.Select(aDirectory => new ImageDirectoryInfo(
				_imageFileExtensionService,
				_imageNormalizerService,
				_logger,
				Path.Combine(InputPath, aDirectory.Name),
				Path.Combine(OutputPath, aDirectory.Name)))
			.ToList();

		_fileSystemInfoCollection.AddRange(subDirectories);
	}

	private void AddFiles(DirectoryInfo directoryInfo)
	{
		var files = directoryInfo
			.GetFiles()
			.Where(aFile => _imageFileExtensionService.ImageFileExtensions.Contains(
								aFile.Extension.ToLowerInvariant()))
			.Select(aFile => new ImageFileInfo(
				_imageNormalizerService,
				_logger,
				Path.Combine(InputPath, aFile.Name),
				Path.Combine(
					OutputPath,
					$"{Path.GetFileNameWithoutExtension(aFile.Name)}{_imageFileExtensionService.OutputImageFileExtension}")))
			.ToList();

		_fileSystemInfoCollection.AddRange(files);
	}

	#endregion
}
