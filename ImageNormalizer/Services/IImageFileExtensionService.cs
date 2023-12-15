using System.Collections.Generic;

namespace ImageNormalizer.Services;

public interface IImageFileExtensionService
{
	HashSet<string> ImageFileExtensions { get; }

	string OutputImageFileExtension { get; }
}
