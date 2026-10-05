using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageNormalizer.Logger;
using ImageNormalizer.Services;

namespace ImageNormalizer.ImageFileSystem;

public class ImageDirectory : IImageDirectory
{
	public ImageDirectory(
		IImageFileExtensionService imageFileExtensionService,
		IImageDataService imageDataService,
		IImageNormalizerService imageNormalizerService,
		IDirectoryService directoryService,
		ILogger logger,
		Arguments arguments,
		CancellationTokenSource cancellationTokenSource)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageDataService = imageDataService;
		_imageNormalizerService = imageNormalizerService;
		_directoryService = directoryService;
		_logger = logger;

		_arguments = arguments;

		_cancellationTokenSource = cancellationTokenSource;

		_imageFiles = [];
		_imageSubDirectories = [];
	}

	public ExitCode BuildImageDirectory()
	{
		if (_cancellationTokenSource.IsCancellationRequested)
		{
			return ExitCode.Aborted;
		}

		try
		{
			var fileNames = _directoryService.GetFileNames(
				_arguments.InputPath);
			var subDirectoryNames = _directoryService.GetSubDirectoryNames(
				_arguments.InputPath);

			_imageFiles = GetImageFiles(fileNames);
			_imageSubDirectories = GetImageSubDirectories(subDirectoryNames);

			foreach (var anImageSubDirectory in _imageSubDirectories)
			{
				var exitCode = anImageSubDirectory.BuildImageDirectory();
				if (exitCode == ExitCode.Aborted)
				{
					return ExitCode.Aborted;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}

		return ExitCode.Successful;
	}

	public ExitCode NormalizeImages()
	{
		if (_cancellationTokenSource.IsCancellationRequested)
		{
			return ExitCode.Aborted;
		}

		try
		{
			if (_imageFiles.Any())
			{
				_directoryService.CreateDirectory(_arguments.OutputPath);

				var exitCode = NormalizeImagesInCurrentDirectory();
				if (exitCode == ExitCode.Aborted)
				{
					return ExitCode.Aborted;
				}
			}

			foreach (var anImageSubDirectory in _imageSubDirectories)
			{
				var exitCode = anImageSubDirectory.NormalizeImages();
				if (exitCode == ExitCode.Aborted)
				{
					return ExitCode.Aborted;
				}
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}

		return ExitCode.Successful;
	}

	private static readonly HashSet<string> ExcludedDirectoryNames = ["__MACOSX"];

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IImageDataService _imageDataService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	private readonly Arguments _arguments;

	private readonly CancellationTokenSource _cancellationTokenSource;

	private IReadOnlyList<IImageFile> _imageFiles;
	private IReadOnlyList<IImageDirectory> _imageSubDirectories;

	private ExitCode NormalizeImagesInCurrentDirectory()
	{
		if (_cancellationTokenSource.IsCancellationRequested)
		{
			return ExitCode.Aborted;
		}

		var maxImageFilesBatchSize = _arguments.MaxDegreeOfParallelism;

		_logger.Info(
			$@"Processing images from input directory ""{_arguments.InputPath}"" to output directory ""{_arguments.OutputPath}"".");

		var imageFileCollections = _imageFiles
			.Chunk(maxImageFilesBatchSize)
			.ToArray();

		foreach (var anImageFileCollection in imageFileCollections)
		{
			if (_cancellationTokenSource.IsCancellationRequested)
			{
				return ExitCode.Aborted;
			}

			try
			{
				ReadImagesFromDisc(anImageFileCollection);

				NormalizeImages(anImageFileCollection);

				WriteImagesToDisc(anImageFileCollection);
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
			finally
			{
				DisposeImages(anImageFileCollection);
			}
		}

		return ExitCode.Successful;
	}

	private static void ReadImagesFromDisc(
		IReadOnlyList<IImageFile> anImageFileCollection)
	{
		foreach (var anImageFile in anImageFileCollection)
		{
			anImageFile.ReadImageFromDisc();
		}
	}

	private static void NormalizeImages(
		IReadOnlyList<IImageFile> anImageFileCollection)
	{
		var imageFileNormalizationTasks = anImageFileCollection
			.Select(anImageFile => new Task(anImageFile.NormalizeImage))
			.ToArray();

		foreach (var anImageFileNormalizationTask in
		         imageFileNormalizationTasks)
		{
			anImageFileNormalizationTask.Start();
		}

		Task.WaitAll(imageFileNormalizationTasks);
	}

	private static void WriteImagesToDisc(
		IReadOnlyList<IImageFile> anImageFileCollection)
	{
		foreach (var anImageFile in anImageFileCollection)
		{
			anImageFile.WriteImageToDisc();
		}
	}

	private static void DisposeImages(
		IReadOnlyList<IImageFile> anImageFileCollection)
	{
		foreach (var anImageFile in anImageFileCollection)
		{
			anImageFile.Dispose();
		}
	}

	private IReadOnlyList<ImageFile> GetImageFiles(
		IReadOnlyList<string> fileNames)
	{
		var imageFiles = fileNames
			.Where(aFileName => _imageFileExtensionService
									.IsSupportedImageFileExtension(
										Path.GetExtension(aFileName)))
			.OrderBy(anImageFileName => anImageFileName)
			.Select(anImageFileName => new ImageFile(
				_imageDataService,
				_imageNormalizerService,
				_arguments with
				{
					InputPath = Path.Combine(_arguments.InputPath, anImageFileName),
					OutputPath = Path.Combine(
						_arguments.OutputPath,
						$"{Path.GetFileNameWithoutExtension(anImageFileName)}{_imageFileExtensionService.OutputImageFileExtension}")
				})
			)
			.Where(anImageFile => !anImageFile.ExistsOutputImageOnDisc())
			.ToList();

		return imageFiles;
	}

	private IReadOnlyList<ImageDirectory> GetImageSubDirectories(
		IReadOnlyList<string> subDirectoryNames)
	{
		var imageSubDirectories = subDirectoryNames
			.Where(aDirectoryName =>
				!ExcludedDirectoryNames.Contains(aDirectoryName))
			.OrderBy(aDirectoryName => aDirectoryName)
			.Select(aDirectoryName => new ImageDirectory(
				_imageFileExtensionService,
				_imageDataService,
				_imageNormalizerService,
				_directoryService,
				_logger,
				_arguments with
				{
					InputPath = Path.Combine(
						_arguments.InputPath, aDirectoryName),
					OutputPath = Path.Combine(
						_arguments.OutputPath, aDirectoryName)
				},
				_cancellationTokenSource))
			.ToList();

		return imageSubDirectories;
	}
}
