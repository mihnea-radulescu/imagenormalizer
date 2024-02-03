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
		Arguments arguments)
		: base(logger, arguments)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;

		_fileSystemInfoCollection = new List<IFileSystemInfo>();
	}

	#region Protected

	protected override void BuildFileSystemInfoSpecific()
	{
		var directoryInfo = new DirectoryInfo(Arguments.InputPath);

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
			$@"Processing input directory ""{Arguments.InputPath}"" into output directory ""{Arguments.OutputPath}"" resizing to output maximum image width/height ""{Arguments.OutputMaximumImageSize}"" at output image quality ""{Arguments.OutputImageQuality}"".");
		
		if (!Directory.Exists(Arguments.OutputPath))
		{
			Directory.CreateDirectory(Arguments.OutputPath);
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
				new Arguments(
					Path.Combine(Arguments.InputPath, aFile.Name),
					Path.Combine(
						Arguments.OutputPath,
						$"{Path.GetFileNameWithoutExtension(aFile.Name)}{_imageFileExtensionService.OutputImageFileExtension}"),
					Arguments.OutputMaximumImageSize,
					Arguments.OutputImageQuality)
				)
			)
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
				new Arguments(
					Path.Combine(Arguments.InputPath, aDirectory.Name),
					Path.Combine(Arguments.OutputPath, aDirectory.Name),
					Arguments.OutputMaximumImageSize,
					Arguments.OutputImageQuality)
				)
			)
			.ToList();

		_fileSystemInfoCollection.AddRange(subDirectories);
	}

	#endregion
}
