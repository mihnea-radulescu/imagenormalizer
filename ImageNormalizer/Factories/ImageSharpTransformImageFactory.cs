using ImageNormalizer.Adapters;

namespace ImageNormalizer.Factories;

public class ImageSharpTransformImageFactory : ITransformImageFactory
{
	public ImageSharpTransformImageFactory(int outputImageQuality)
    {
		_outputImageQuality = outputImageQuality;
	}

    public ITransformImage GetTransformImage(string inputFileName, string outputFileName)
		=> new ImageSharpTransformImage(inputFileName, outputFileName, _outputImageQuality);

	#region Private

	private readonly int _outputImageQuality;

	#endregion
}
