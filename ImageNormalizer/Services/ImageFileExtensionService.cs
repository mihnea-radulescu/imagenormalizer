using System;
using System.Collections.Generic;

namespace ImageNormalizer.Services;

public class ImageFileExtensionService : IImageFileExtensionService
{
    public ImageFileExtensionService()
    {
        _imageFileExtensions = new HashSet<string>(
        [
			".bmp",
			".gif",
			".ico",
			".jpg", ".jpe", ".jpeg",
			".png",
			".tif", ".tiff",
			".webp"
		],
		StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> ImageFileExtensions => _imageFileExtensions;

	public string OutputImageFileExtension => ".jpg";

    #region Private

    private readonly HashSet<string> _imageFileExtensions;

	#endregion
}
