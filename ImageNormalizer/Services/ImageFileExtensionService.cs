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
			".cr2",
			".cur",
			".dds",
			".dng",
			".exr",
			".fts",
			".gif",
			".hdr",
			".heic",
			".heif",
			".ico",
			".jfif",
			".jp2",
			".jpe", ".jpeg", ".jpg",
			".jps",
			".mng",
			".nef",
			".nrw",
			".orf",
			".pam",
			".pbm",
			".pcd",
			".pcx",
			".pef",
			".pes",
			".pfm",
			".pgm",
			".picon",
			".pict",
			".png",
			".ppm",
			".psd",
			".qoi",
			".raf",
			".rw2",
			".sgi",
			".svg",
			".tga",
			".tif", ".tiff",
			".wbmp",
			".webp",
			".xbm",
			".xpm"
		],
		StringComparer.InvariantCultureIgnoreCase);
	}

	public HashSet<string> ImageFileExtensions => _imageFileExtensions;

	public string OutputImageFileExtension => ".jpg";

	#region Private

	private readonly HashSet<string> _imageFileExtensions;

	#endregion
}
