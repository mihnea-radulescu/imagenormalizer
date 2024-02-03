using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageNormalizer.Exceptions;

namespace ImageNormalizer.Adapters;

public class ImageSharpImageTransformer : IImageTransformer
{
	public void TransformImage(
		string inputFileName, string outputFileName, int outputImageQuality)
	{
		using (var loadedImage = LoadImage(inputFileName))
		{
			ClearMetadata(loadedImage);
			SaveImage(loadedImage, inputFileName, outputFileName, outputImageQuality);
		}
	}

	#region Private

	private static Image LoadImage(string inputFileName)
	{
		try
		{
			var loadedImage = Image.Load(inputFileName);

			return loadedImage;
		}
		catch (Exception ex)
		{
			throw new TransformImageException(@$"Could not load image ""{inputFileName}"".", ex);
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

	private static void SaveImage(
		Image loadedImage, string inputFileName, string outputFileName, int outputImageQuality)
	{
		IImageEncoder encoder = new JpegEncoder
		{
			Quality = outputImageQuality
		};

		try
		{
			loadedImage!.Save(outputFileName, encoder);
		}
		catch (Exception ex)
		{
			throw new TransformImageException(
				@$"Could not save image ""{inputFileName}"" to ""{outputFileName}"".", ex);
		}
	}

	#endregion
}
