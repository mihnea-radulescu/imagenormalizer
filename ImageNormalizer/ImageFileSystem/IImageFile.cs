using System;

namespace ImageNormalizer.ImageFileSystem;

public interface IImageFile : IDisposable
{
	void ReadImageFromDisc();

	void NormalizeImage();

	void WriteImageToDisc();
}
