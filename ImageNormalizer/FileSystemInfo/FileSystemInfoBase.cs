using System;
using ImageNormalizer.Exceptions;
using ImageNormalizer.Logger;

namespace ImageNormalizer.FileSystemInfo;

public abstract class FileSystemInfoBase : IFileSystemInfo
{
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
	protected readonly Arguments Arguments;

	protected FileSystemInfoBase(
		ILogger logger,
		Arguments arguments)
	{
		Logger = logger;
		Arguments = arguments;

		if (Arguments.InputPath.Equals(
				Arguments.OutputPath, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new ArgumentException(
				$@"Input path ""{Arguments.InputPath}"" must be different than output path ""{Arguments.OutputPath}"".");
		}
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
