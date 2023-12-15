using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageNormalizer.Exceptions;

namespace ImageNormalizer.Adapters;

public class ImageSharpTransformImage : ITransformImage
{
	public ImageSharpTransformImage(
		string inputFileName, string outputFileName, int outputImageQuality)
    {
		InputFileName = inputFileName;
		OutputFileName = outputFileName;
		OutputImageQuality = outputImageQuality;
	}

	public string InputFileName { get; }
	public string OutputFileName { get; }
	public int OutputImageQuality { get; }

	public void SaveTransformedImage()
	{
		ThrowExceptionIfDisposed();

		LoadImageIfNecessary();

		ClearMetadata();
		SaveImage();
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			if (_loadedImage is not null)
			{
				_loadedImage.Dispose();
				_loadedImage = null;
			}

			_isDisposed = true;
		}
	}

	#region Private

	private bool _isDisposed;
	private Image? _loadedImage;

	private void ThrowExceptionIfDisposed()
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException(nameof(ImageSharpTransformImage));
		}
	}

	private void LoadImageIfNecessary()
	{
		if (_loadedImage is null)
		{
			try
			{
				_loadedImage = Image.Load(InputFileName);
			}
			catch (Exception ex)
			{
				throw new TransformImageException(@$"Could not load image ""{InputFileName}"".", ex);
			}
		}
	}

	private void ClearMetadata()
	{
		var imageMetadata = _loadedImage!.Metadata;

		imageMetadata.CicpProfile = null;
		imageMetadata.ExifProfile = null;
		imageMetadata.IccProfile = null;
		imageMetadata.IptcProfile = null;
		imageMetadata.XmpProfile = null;
	}

	private void SaveImage()
	{
		IImageEncoder encoder = new JpegEncoder
		{
			Quality = OutputImageQuality
		};

		try
		{
			_loadedImage!.Save(OutputFileName, encoder);
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not save image ""{InputFileName}"" to ""{OutputFileName}"".", ex);
		}
	}

	#endregion
}
