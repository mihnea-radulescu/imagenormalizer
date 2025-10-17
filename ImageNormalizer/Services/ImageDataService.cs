using System.IO;
using ImageNormalizer.Extensions;
using ImageNormalizer.Logger;

namespace ImageNormalizer.Services;

public class ImageDataService : IImageDataService
{
	public ImageDataService(ILogger logger)
	{
		_logger = logger;
	}

	public Stream? ReadImageDataFromDisc(Arguments arguments)
	{
		Stream? inputImageDataStream = default;

		try
		{
			var inputImageFileContent = File.ReadAllBytes(arguments.InputPath);

			inputImageDataStream = new MemoryStream(inputImageFileContent);
			inputImageDataStream.Reset();
		}
		catch
		{
			_logger.Error(@$"Could not read image file ""{arguments.InputPath}"" from disc.");
		}

		return inputImageDataStream;
	}

	public void WriteImageDataToDisc(Stream outputImageDataStream, Arguments arguments)
	{
		try
		{
			using var outputImageFileStream = File.Create(arguments.OutputPath);
			outputImageDataStream.CopyTo(outputImageFileStream);
		}
		catch
		{
			_logger.Error(@$"Could not write image file ""{arguments.OutputPath}"" to disc.");
		}
	}

	#region Private

	private readonly ILogger _logger;

	#endregion
}
