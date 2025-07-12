using System;
using ImageMagick;
using ImageNormalizer.Exceptions;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;

namespace ImageNormalizer.Adapters;

public class ImageTransformer : IImageTransformer
{
	public ImageTransformer(
		IImageResizeCalculator imageResizeCalculator, ILogger logger)
	{
		_imageResizeCalculator = imageResizeCalculator;
		_logger = logger;
	}

	public void TransformImage(Arguments arguments)
	{
		try
		{
			using (var loadedImage = LoadImage(arguments))
			{
				ApplyImageOrientation(loadedImage);
				ClearMetadata(loadedImage);
				ResizeImage(loadedImage, arguments);
				SaveImage(loadedImage, arguments);
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	#region Private

	private readonly IImageResizeCalculator _imageResizeCalculator;
	private readonly ILogger _logger;

	private static IMagickImage LoadImage(Arguments arguments)
	{
		try
		{
			var loadedImage = new MagickImage(arguments.InputPath);

			return loadedImage;
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not load image ""{arguments.InputPath}"".", ex);
		}
	}

	private static void ApplyImageOrientation(IMagickImage loadedImage) => loadedImage.AutoOrient();

	private static void ClearMetadata(IMagickImage loadedImage)
	{
		var exifProfile = loadedImage.GetExifProfile();
		var iptcProfile = loadedImage.GetIptcProfile();
		var xmpProfile = loadedImage.GetXmpProfile();

		if (exifProfile is not null)
		{
			loadedImage.RemoveProfile(exifProfile);
		}

		if (iptcProfile is not null)
		{
			loadedImage.RemoveProfile(iptcProfile);
		}

		if (xmpProfile is not null)
		{
			loadedImage.RemoveProfile(xmpProfile);
		}
	}

	private void ResizeImage(IMagickImage loadedImage, Arguments arguments)
	{
		var loadedImageSize = new ImageSize((int)loadedImage.Width, (int)loadedImage.Height);

		if (!_imageResizeCalculator.ShouldResize(loadedImageSize, arguments))
		{
			return;
		}

		var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(
			loadedImageSize, arguments);

		try
		{
			loadedImage.Resize((uint)resizedImageSize.Width, (uint)resizedImageSize.Height);
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not resize image ""{arguments.InputPath}"" to {resizedImageSize.Width} x {resizedImageSize.Height}.", ex);
		}
	}

	private static void SaveImage(IMagickImage loadedImage, Arguments arguments)
	{
		loadedImage.Quality = (uint)arguments.OutputImageQuality;

		try
		{
			loadedImage.Write(arguments.OutputPath, MagickFormat.Jpg);
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not save image ""{arguments.InputPath}"" to ""{arguments.OutputPath}"".", ex);
		}
	}

	#endregion
}
