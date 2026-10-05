using System;
using System.Collections.Generic;

namespace ImageNormalizer.Services;

public class ImageFileExtensionService : IImageFileExtensionService
{
	public bool IsSupportedImageFileExtension(string fileExtension)
		=> SupportedImageFileExtensions.Contains(fileExtension);

	public string OutputImageFileExtension => ".jpg";

	private static readonly HashSet<string> SupportedImageFileExtensions =
		new(
		[
			".avif",
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
			".jxl",
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
