using System;
using ImageNormalizer.Exceptions;
using ImageNormalizer.Logger;

namespace ImageNormalizer.FileSystemInfo;

public abstract class FileSystemInfoBase : IFileSystemInfo
{
	public string InputPath { get; }
	public string OutputPath { get; }
	public int OutputImageQuality { get; }

	public void BuildFileSystemInfo()
	{
		ExecuteAction(BuildFileSystemInfoSpecific);

		_hasBuiltFileSystemInfo = true;
	}

	public void NormalizeFileSystemInfo()
	{
		if (!_hasBuiltFileSystemInfo)
		{
			throw new TransformImageException(
				$"{nameof(BuildFileSystemInfo)} needs to be called prior to {nameof(NormalizeFileSystemInfo)}.");
		}

		ExecuteAction(NormalizeFileSystemInfoSpecific);
	}

	#region Protected

	protected readonly ILogger Logger;

	protected FileSystemInfoBase(
		ILogger logger,
		string inputPath,
		string outputPath,
		int outputImageQuality)
	{
		if (inputPath.Equals(outputPath, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new ArgumentException(
				$@"Input path ""{inputPath}"" must be different than output path ""{outputPath}"".");
		}

		Logger = logger;

		InputPath = inputPath;
		OutputPath = outputPath;
		OutputImageQuality = outputImageQuality;
	}

	protected abstract void BuildFileSystemInfoSpecific();
	protected abstract void NormalizeFileSystemInfoSpecific();

	#endregion

	#region Private

	private bool _hasBuiltFileSystemInfo;

	private void ExecuteAction(Action action)
	{
		try
		{
			action();
		}
		catch (TransformImageException tEx)
		{
			Logger.Error(tEx);
		}
	}

	#endregion
}
