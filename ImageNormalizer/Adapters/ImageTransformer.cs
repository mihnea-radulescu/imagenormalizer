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
		using var image = new MagickImage(inputImageDataStream, magickFormat);

		ApplyImageOrientation(image);
		ResizeImage(image, arguments);

		if (arguments.ShouldRemoveImageProfileData)
		{
			RemoveImageProfileData(image);
		}

		var outputImageDataStream = SaveImage(image, arguments);
		return outputImageDataStream;
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

	private static void ApplyImageOrientation(IMagickImage image)
		=> image.AutoOrient();

	private static void RemoveImageProfileData(IMagickImage image)
	{
		var exifProfile = image.GetExifProfile();
		var iptcProfile = image.GetIptcProfile();
		var xmpProfile = image.GetXmpProfile();
		var colorProfile = image.GetColorProfile();

		if (exifProfile is not null)
		{
			image.RemoveProfile(exifProfile);
		}

		if (iptcProfile is not null)
		{
			image.RemoveProfile(iptcProfile);
		}

		if (xmpProfile is not null)
		{
			image.RemoveProfile(xmpProfile);
		}

		if (colorProfile is not null)
		{
			image.RemoveProfile(colorProfile);
		}
	}

	private void ResizeImage(IMagickImage image, Arguments arguments)
	{
		var imageSize = new ImageSize((int)image.Width, (int)image.Height);

		if (_imageResizeCalculator.ShouldResize(imageSize, arguments))
		{
			var resizedImageSize = _imageResizeCalculator.GetResizedImageSize(
				imageSize, arguments);

			image.Resize(
				(uint)resizedImageSize.Width, (uint)resizedImageSize.Height);
		}
	}

	private static Stream SaveImage(IMagickImage image, Arguments arguments)
	{
		image.Quality = (uint)arguments.OutputImageQuality;

		var outputImageDataStream = new MemoryStream();
		image.Write(outputImageDataStream, MagickFormat.Jpg);
		outputImageDataStream.Reset();

		return outputImageDataStream;
	}
}
