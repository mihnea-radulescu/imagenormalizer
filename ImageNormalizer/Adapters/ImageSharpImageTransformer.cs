using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageNormalizer.Exceptions;

namespace ImageNormalizer.Adapters;

public class ImageSharpImageTransformer : IImageTransformer
{
	public void TransformImage(Arguments arguments)
	{
		using (var loadedImage = LoadImage(arguments))
		{
			ClearMetadata(loadedImage);
			SaveImage(loadedImage, arguments);
		}
	}

	#region Private

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
