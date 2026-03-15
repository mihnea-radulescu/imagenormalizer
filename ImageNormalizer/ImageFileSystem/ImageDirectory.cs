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
			var subDirectories = _directoryService.GetSubDirectories(
				_arguments.InputPath);

			_imageFiles = GetImageFiles(files);
			_imageSubDirectories = GetImageSubDirectories(subDirectories);

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
			if (HasImageFiles)
			{
				_directoryService.CreateDirectory(_arguments.OutputPath);

				NormalizeImagesInCurrentDirectory();
			}

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

	private IReadOnlyList<IImageFile> _imageFiles;
	private IReadOnlyList<IImageDirectory> _imageSubDirectories;

	private void NormalizeImagesInCurrentDirectory()
	{
		var maxImageFilesBatchSize = _arguments.MaxDegreeOfParallelism;

		_logger.Info(
			$@"Processing images from input directory ""{_arguments.InputPath}"" to output directory ""{_arguments.OutputPath}"".");

		var imageFileCollections = _imageFiles
			.Chunk(maxImageFilesBatchSize)
			.ToArray();

		foreach (var anImageFileCollection in imageFileCollections)
		{
			try
			{
				var imageFileNormalizationTasks = anImageFileCollection
					.Select(anImageFile => new Task(anImageFile.NormalizeImage))
					.ToArray();

				foreach (var anImageFile in anImageFileCollection)
				{
					anImageFile.ReadImageFromDisc();
				}

				foreach (var anImageFileNormalizationTask in
							 imageFileNormalizationTasks)
				{
					anImageFileNormalizationTask.Start();
				}

				Task.WaitAll(imageFileNormalizationTasks);

				foreach (var anImageFile in anImageFileCollection)
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
				foreach (var anImageFile in anImageFileCollection)
				{
					anImageFile.Dispose();
				}
			}
		}
	}

	private IReadOnlyList<ImageFile> GetImageFiles(IReadOnlyList<string> files)
	{
		var imageFiles = files
			.Where(aFile => _imageFileExtensionService
								.ImageFileExtensions
								.Contains(Path.GetExtension(aFile)))
			.OrderBy(anImageFile => anImageFile)
			.Select(anImageFile => new ImageFile(
				_imageDataService,
				_imageNormalizerService,
				new Arguments(
					Path.Combine(_arguments.InputPath, anImageFile),
					Path.Combine(
						_arguments.OutputPath,
						$"{Path.GetFileNameWithoutExtension(anImageFile)}{_imageFileExtensionService.OutputImageFileExtension}"),
					_arguments.OutputMaximumImageSize,
					_arguments.OutputImageQuality,
					_arguments.ShouldRemoveImageProfileData,
					_arguments.MaxDegreeOfParallelism)
				)
			)
			.ToList();

		return imageFiles;
	}

	private IReadOnlyList<ImageDirectory> GetImageSubDirectories(
		IReadOnlyList<string> subDirectories)
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

		return imageSubDirectories;
	}

	private bool HasImageFiles => _imageFiles.Any();
}
