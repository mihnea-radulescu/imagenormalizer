using System;

namespace ImageNormalizer.ImageResizing;

public class ImageResizeCalculator : IImageResizeCalculator
{
	public bool ShouldResize(ImageSize imageSize, Arguments arguments)
	{
		var imageMaxSize = GetImageMaxSize(imageSize);

		var shouldResize = (imageMaxSize > arguments.OutputMaximumImageSize);
		return shouldResize;
	}

	public ImageSize GetResizedImageSize(ImageSize imageSize, Arguments arguments)
	{
		if (!ShouldResize(imageSize, arguments))
		{
			return imageSize;
		}

		var imageMaxSize = GetImageMaxSize(imageSize);

		var resizeRatio = (double)arguments.OutputMaximumImageSize / (double)imageMaxSize;

		var resizedImageWidth = (double)imageSize.Width * resizeRatio;
		var resizedImageHeight = (double)imageSize.Height * resizeRatio;

		var resizedImageSize = new ImageSize((int)resizedImageWidth, (int)resizedImageHeight);
		return resizedImageSize;
	}

	#region Private

	private static int GetImageMaxSize(ImageSize imageSize)
		=> Math.Max(imageSize.Width, imageSize.Height);

	#endregion
}
