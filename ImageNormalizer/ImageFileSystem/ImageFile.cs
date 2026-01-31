using System;
using System.IO;
using ImageNormalizer.Services;

namespace ImageNormalizer.ImageFileSystem;

public class ImageFile : IImageFile
{
	public ImageFile(
		IImageDataService imageDataService,
		IImageNormalizerService imageNormalizerService,
		Arguments arguments)
	{
		_imageDataService = imageDataService;
		_imageNormalizerService = imageNormalizerService;

		_arguments = arguments;
	}

	public void ReadImageFromDisc()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		_inputImageDataStream = _imageDataService.ReadImageDataFromDisc(
			_arguments);
	}

	public void NormalizeImage()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		if (_inputImageDataStream is not null)
		{
			_outputImageDataStream = _imageNormalizerService.NormalizeImage(
				_inputImageDataStream, _arguments);
		}
	}

	public void WriteImageToDisc()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		if (_outputImageDataStream is not null)
		{
			_imageDataService.WriteImageDataToDisc(
				_outputImageDataStream, _arguments);
		}
	}

	public void Dispose()
	{
		if (!_hasBeenDisposed)
		{
			_inputImageDataStream?.Dispose();
			_outputImageDataStream?.Dispose();

			_inputImageDataStream = null;
			_outputImageDataStream = null;

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	private readonly IImageDataService _imageDataService;
	private readonly IImageNormalizerService _imageNormalizerService;

	private readonly Arguments _arguments;

	private Stream? _inputImageDataStream;
	private Stream? _outputImageDataStream;

	private bool _hasBeenDisposed;

	private void ThrowObjectDisposedExceptionIfNecessary()
		=> ObjectDisposedException.ThrowIf(_hasBeenDisposed, this);
}
