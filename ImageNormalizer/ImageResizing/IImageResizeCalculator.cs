namespace ImageNormalizer.ImageResizing;

public interface IImageResizeCalculator
{
	bool ShouldResize(ImageSize imageSize, Arguments arguments);

	ImageSize GetResizedImageSize(ImageSize imageSize, Arguments arguments);
}
