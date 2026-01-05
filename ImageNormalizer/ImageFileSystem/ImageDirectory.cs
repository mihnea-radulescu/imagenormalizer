using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		Arguments arguments)
	{
		_imageFileExtensionService = imageFileExtensionService;
		_imageDataService = imageDataService;
		_imageNormalizerService = imageNormalizerService;
		_directoryService = directoryService;
		_logger = logger;

		_arguments = arguments;

		_imageFiles = [];
		_imageSubDirectories = [];
	}

	public void BuildImageDirectory()
	{
		try
		{
			var files = _directoryService.GetFiles(_arguments.InputPath);
			var subDirectories = _directoryService.GetSubDirectories(_arguments.InputPath);

			AddFiles(files);
			AddSubDirectories(subDirectories);

			foreach (var anImageSubDirectory in _imageSubDirectories)
			{
				anImageSubDirectory.BuildImageDirectory();
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	public void NormalizeImages()
	{
		try
		{
			_directoryService.CreateDirectory(_arguments.OutputPath);

			NormalizeImagesInCurrentDirectory();

			foreach (var anImageSubDirectory in _imageSubDirectories)
			{
				anImageSubDirectory.NormalizeImages();
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	private static readonly HashSet<string> ExcludedDirectories = ["__MACOSX"];

	private readonly IImageFileExtensionService _imageFileExtensionService;
	private readonly IImageNormalizerService _imageNormalizerService;
	private readonly IImageDataService _imageDataService;
	private readonly IDirectoryService _directoryService;
	private readonly ILogger _logger;

	private readonly Arguments _arguments;

	private readonly List<IImageFile> _imageFiles;
	private readonly List<IImageDirectory> _imageSubDirectories;

	private void NormalizeImagesInCurrentDirectory()
	{
		var imageFilesCount = _imageFiles.Count;
		var maxImageFilesBatchSize = _arguments.MaxDegreeOfParallelism;

		_logger.Info(
			$@"Processing images from input directory ""{_arguments.InputPath}"" to output directory ""{_arguments.OutputPath}"".");

		for (var imageFilesIndex = 0; imageFilesIndex < imageFilesCount; imageFilesIndex += maxImageFilesBatchSize)
		{
			var imageFilesBatchSize = Math.Min(maxImageFilesBatchSize, imageFilesCount - imageFilesIndex);

			var imageFilesBatch = _imageFiles
				.Skip(imageFilesIndex)
				.Take(imageFilesBatchSize)
				.ToList();

			try
			{
				var imageFileNormalizationTasks = new Task[imageFilesBatchSize];

				for (var imageFilesBatchIndex = 0; imageFilesBatchIndex < imageFilesBatchSize; imageFilesBatchIndex++)
				{
					var currentImageFile = imageFilesBatch[imageFilesBatchIndex];

					imageFileNormalizationTasks[imageFilesBatchIndex] = new Task(currentImageFile.NormalizeImage);
				}

				foreach (var anImageFile in imageFilesBatch)
				{
					anImageFile.ReadImageFromDisc();
				}

				foreach (var anImageFileNormalizationTask in imageFileNormalizationTasks)
				{
					anImageFileNormalizationTask.Start();
				}

				Task.WaitAll(imageFileNormalizationTasks);

				foreach (var anImageFile in imageFilesBatch)
				{
					anImageFile.WriteImageToDisc();
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
			finally
			{
				foreach (var anImageFile in imageFilesBatch)
				{
					anImageFile.Dispose();
				}
			}
		}
	}

	private void AddFiles(IReadOnlyList<string> files)
	{
		var imageFiles = files
			.Where(aFile => _imageFileExtensionService.ImageFileExtensions.Contains(Path.GetExtension(aFile)))
			.OrderBy(aFile => aFile)
			.Select(aFile => new ImageFile(
				_imageDataService,
				_imageNormalizerService,
				new Arguments(
					Path.Combine(_arguments.InputPath, aFile),
					Path.Combine(
						_arguments.OutputPath,
						$"{Path.GetFileNameWithoutExtension(aFile)}{_imageFileExtensionService.OutputImageFileExtension}"),
					_arguments.OutputMaximumImageSize,
					_arguments.OutputImageQuality,
					_arguments.ShouldRemoveImageProfileData,
					_arguments.MaxDegreeOfParallelism)
				)
			)
			.ToList();

		_imageFiles.AddRange(imageFiles);
	}

	private void AddSubDirectories(IReadOnlyList<string> subDirectories)
	{
		var imageSubDirectories = subDirectories
			.Where(aDirectory => !ExcludedDirectories.Contains(aDirectory))
			.OrderBy(aDirectory => aDirectory)
			.Select(aDirectory => new ImageDirectory(
				_imageFileExtensionService,
				_imageDataService,
				_imageNormalizerService,
				_directoryService,
				_logger,
				new Arguments(
					Path.Combine(_arguments.InputPath, aDirectory),
					Path.Combine(_arguments.OutputPath, aDirectory),
					_arguments.OutputMaximumImageSize,
					_arguments.OutputImageQuality,
					_arguments.ShouldRemoveImageProfileData,
					_arguments.MaxDegreeOfParallelism)
				)
			)
			.ToList();

		_imageSubDirectories.AddRange(imageSubDirectories);
	}
}
