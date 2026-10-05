using System;

namespace ImageNormalizer.ImageFileSystem;

public interface IImageFile : IDisposable
{
	bool ExistsOutputImageOnDisc();

	void ReadImageFromDisc();

	void NormalizeImage();

	void WriteImageToDisc();
}
