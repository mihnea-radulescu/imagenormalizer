namespace ImageNormalizer.Services;

public interface IImageFileExtensionService
{
	bool IsSupportedImageFileExtension(string fileExtension);

	string OutputImageFileExtension { get; }
}
