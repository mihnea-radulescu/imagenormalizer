using System.IO;
using ImageMagick;
using ImageNormalizer.Extensions;
using ImageNormalizer.ImageResizing;

namespace ImageNormalizer.Adapters;

public class ImageTransformer : IImageTransformer
{
	public ImageTransformer(IImageResizeCalculator imageResizeCalculator)
	{
		_imageResizeCalculator = imageResizeCalculator;
	}

	public Stream TransformImage(
		Stream inputImageDataStream, Arguments arguments)
	{
		var magickFormat = GetMagickFormat(arguments.InputPath);

		using (var loadedImage = new MagickImage(
					inputImageDataStream, magickFormat))
		{
			ApplyImageOrientation(loadedImage);
			ResizeImage(loadedImage, arguments);

			if (arguments.ShouldRemoveImageProfileData)
			{
				RemoveImageProfileData(loadedImage);
			}

			var outputImageDataStream = SaveImage(loadedImage, arguments);
			return outputImageDataStream;
		}
	}

	private readonly IImageResizeCalculator _imageResizeCalculator;

	private static MagickFormat GetMagickFormat(string filePath)
	{
		var normalizedFileExtension =
			Path.GetExtension(filePath).ToLowerInvariant();

		return normalizedFileExtension switch
		{
			".cur" => MagickFormat.Cur,
			".dng" => MagickFormat.Dng,
			".ico" => MagickFormat.Ico,
			".nrw" => MagickFormat.Nrw,
			".pef" => MagickFormat.Pef,
			".pict" => MagickFormat.Pict,
			".tga" => MagickFormat.Tga,
			".wbmp" => MagickFormat.Wbmp,
			_ => MagickFormat.Unknown
		};
	}

	private static void ApplyImageOrientation(IMagickImage loadedImage)
		=> loadedImage.AutoOrient();

	private static void RemoveImageProfileData(IMagickImage loadedImage)
	{
		var exifProfile = loadedImage.GetExifProfile();
		var iptcProfile = loadedImage.GetIptcProfile();
		var xmpProfile = loadedImage.GetXmpProfile();
		var colorProfile = loadedImage.GetColorProfile();

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

		if (colorProfile is not null)
		{
			loadedImage.RemoveProfile(colorProfile);
		}
	}

	private void ResizeImage(IMagickImage loadedImage, Arguments arguments)
	{
		var loadedImageSize = new ImageSize(
			(int)loadedImage.Width, (int)loadedImage.Height);

		if (_imageResizeCalculator.ShouldResize(loadedImageSize, arguments))
		{
			var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(
				loadedImageSize, arguments);

			loadedImage.Resize(
				(uint)resizedImageSize.Width, (uint)resizedImageSize.Height);
		}
	}

	private static Stream SaveImage(
		IMagickImage loadedImage, Arguments arguments)
	{
		loadedImage.Quality = (uint)arguments.OutputImageQuality;

		var outputImageDataStream = new MemoryStream();
		loadedImage.Write(outputImageDataStream, MagickFormat.Jpg);
		outputImageDataStream.Reset();

		return outputImageDataStream;
	}
}
