using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ImageNormalizer.Exceptions;
using ImageNormalizer.ImageResizing;
using ImageNormalizer.Logger;

namespace ImageNormalizer.Adapters;

public class ImageSharpImageTransformer : IImageTransformer
{
	public ImageSharpImageTransformer(
		IImageResizeCalculator imageResizeCalculator,
		ILogger logger)
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

	private static Image LoadImage(Arguments arguments)
	{
		try
		{
			var loadedImage = Image.Load(arguments.InputPath);

			return loadedImage;
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not load image ""{arguments.InputPath}"".", ex);
		}
	}

	private static void ClearMetadata(Image loadedImage)
	{
		var imageMetadata = loadedImage!.Metadata;

		imageMetadata.CicpProfile = null;
		imageMetadata.ExifProfile = null;
		imageMetadata.IccProfile = null;
		imageMetadata.IptcProfile = null;
		imageMetadata.XmpProfile = null;
	}

	private void ResizeImage(Image loadedImage, Arguments arguments)
	{
		var loadedImageSize = new ImageSize(loadedImage.Width, loadedImage.Height);
		
		if (!_imageResizeCalculator.ShouldResize(loadedImageSize, arguments))
		{
			return;
		}

		var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(
			loadedImageSize, arguments);

		try
		{
			loadedImage.Mutate(context => context.Resize(
				resizedImageSize.Width, resizedImageSize.Height));
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not resize image ""{arguments.InputPath}"" to {resizedImageSize.Width} x {resizedImageSize.Height}.", ex);
		}
	}

	private static void SaveImage(Image loadedImage, Arguments arguments)
	{
		IImageEncoder encoder = new JpegEncoder
		{
			Quality = arguments.OutputImageQuality
		};

		try
		{
			loadedImage!.Save(arguments.OutputPath, encoder);
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not save image ""{arguments.InputPath}"" to ""{arguments.OutputPath}"".", ex);
		}
	}

	#endregion
}
