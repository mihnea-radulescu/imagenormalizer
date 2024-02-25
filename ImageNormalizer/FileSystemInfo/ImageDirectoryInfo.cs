using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.FileSystemInfo;

public class ImageDirectoryInfo : IImageFileSystemInfo
{
	static ImageDirectoryInfo()
	{
		ParallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		};
	}
	
	public ImageDirectoryInfo(
		IImageFileExtensionService imageFileExtensionService,
		IImageNormalizerService imageNormalizerService,
		IDirectoryService directoryService,
		ILogger logger,
		Arguments arguments)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageNormalizerService = imageNormalizerService;
		_directoryService = directoryService;
		_logger = logger;

		_arguments = arguments;

		_imageFileInfoCollection = new List<ImageFileInfo>();
		_imageSubDirectoryInfoCollection = new List<ImageDirectoryInfo>();

		_hasBuiltFileSystemInfo = false;
	}

	public void BuildFileSystemInfo()
	{
		IReadOnlyList<string> files;
		IReadOnlyList<string> subDirectories;

		try
		{
			files = _directoryService.GetFiles(_arguments.InputPath);
			subDirectories = _directoryService.GetSubDirectories(_arguments.InputPath);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);

			return;
		}

		AddFiles(files);
		AddSubDirectories(subDirectories);

		foreach (var anImageSubDirectoryInfo in _imageSubDirectoryInfoCollection)
		{
			anImageSubDirectoryInfo.BuildFileSystemInfo();
		}

		_hasBuiltFileSystemInfo = true;
	}

	public void NormalizeFileSystemInfo()
	{
		if (!_hasBuiltFileSystemInfo)
		{
			_logger.Error(
				$"{nameof(BuildFileSystemInfo)} needs to be called prior to {nameof(NormalizeFileSystemInfo)}.");

			return;
		}

		try
		{
			_directoryService.CreateDirectory(_arguments.OutputPath);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);

			return;
		}

		_logger.Info(
			$@"Processing input directory ""{_arguments.InputPath}"" into output directory ""{_arguments.OutputPath}"" resizing to output maximum image width/height ""{_arguments.OutputMaximumImageSize}"" at output image quality ""{_arguments.OutputImageQuality}"".");

		Parallel.ForEach(
			_imageFileInfoCollection,
			ParallelOptions,
			anImageFileInfo =>
			{
				anImageFileInfo.NormalizeFileSystemInfo();
			});

		foreach (var anImageSubDirectoryInfo in _imageSubDirectoryInfoCollection)
		{
			anImageSubDirectoryInfo.NormalizeFileSystemInfo();
		}
	}

	#region Private

	private static readonly ParallelOptions ParallelOptions;

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	private readonly Arguments _arguments;

	private readonly List<ImageFileInfo> _imageFileInfoCollection;
	private readonly List<ImageDirectoryInfo> _imageSubDirectoryInfoCollection;

	private bool _hasBuiltFileSystemInfo;

	private void AddFiles(IReadOnlyList<string> files)
	{
		var imageFiles = files
			.Where(aFile => _imageFileExtensionService.ImageFileExtensions.Contains(
								Path.GetExtension(aFile)))
			.Select(aFile => new ImageFileInfo(
				_imageNormalizerService,
				new Arguments(
					Path.Combine(_arguments.InputPath, aFile),
					Path.Combine(
						_arguments.OutputPath,
						$"{Path.GetFileNameWithoutExtension(aFile)}{_imageFileExtensionService.OutputImageFileExtension}"),
					_arguments.OutputMaximumImageSize,
					_arguments.OutputImageQuality)
				)
			)
			.ToList();

		_imageFileInfoCollection.AddRange(imageFiles);
	}

	private void AddSubDirectories(IReadOnlyList<string> subDirectories)
	{
		var imageSubDirectories = subDirectories
			.Select(aDirectory => new ImageDirectoryInfo(
				_imageFileExtensionService,
				_imageNormalizerService,
				_directoryService,
				_logger,
				new Arguments(
					Path.Combine(_arguments.InputPath, aDirectory),
					Path.Combine(_arguments.OutputPath, aDirectory),
					_arguments.OutputMaximumImageSize,
					_arguments.OutputImageQuality)
				)
			)
			.ToList();

		_imageSubDirectoryInfoCollection.AddRange(imageSubDirectories);
	}

	#endregion
}
