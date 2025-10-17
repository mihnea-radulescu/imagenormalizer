using System.IO;

namespace ImageNormalizer.Extensions;

public static class StreamExtensions
{
	public static void Reset(this Stream stream) => stream.Position = 0;
}
