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
		string outputPath,
		int outputImageQuality)
		: base(logger, inputPath, outputPath, outputImageQuality)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;

		_fileSystemInfoCollection = new List<IFileSystemInfo>();
	}

	#region Protected

	protected override void BuildFileSystemInfoSpecific()
	{
		var directoryInfo = new DirectoryInfo(InputPath);

		AddFiles(directoryInfo);
		AddSubDirectories(directoryInfo);

		foreach (var aFileSystemInfo in _fileSystemInfoCollection)
		{
			aFileSystemInfo.BuildFileSystemInfo();
		}
	}

	protected override void NormalizeFileSystemInfoSpecific()
	{
		Logger.Info(
			$@"Processing input directory ""{InputPath}"" into output directory ""{OutputPath}"" at output image quality {OutputImageQuality}.");
		
		if (!Directory.Exists(OutputPath))
		{
			Directory.CreateDirectory(OutputPath);
		}
		
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

	private void AddFiles(DirectoryInfo directoryInfo)
	{
		var files = directoryInfo
			.GetFiles()
			.Where(aFile => _imageFileExtensionService.ImageFileExtensions.Contains(
								aFile.Extension.ToLowerInvariant()))
			.Select(aFile => new ImageFileInfo(
				_imageNormalizerService,
				Logger,
				Path.Combine(InputPath, aFile.Name),
				Path.Combine(
					OutputPath,
					$"{Path.GetFileNameWithoutExtension(aFile.Name)}{_imageFileExtensionService.OutputImageFileExtension}"),
				OutputImageQuality))
			.ToList();

		_fileSystemInfoCollection.AddRange(files);
	}

	private void AddSubDirectories(DirectoryInfo directoryInfo)
	{
		var subDirectories = directoryInfo
			.GetDirectories()
			.Select(aDirectory => new ImageDirectoryInfo(
				_imageFileExtensionService,
				_imageNormalizerService,
				Logger,
				Path.Combine(InputPath, aDirectory.Name),
				Path.Combine(OutputPath, aDirectory.Name),
				OutputImageQuality))
			.ToList();

		_fileSystemInfoCollection.AddRange(subDirectories);
	}

	#endregion
}
