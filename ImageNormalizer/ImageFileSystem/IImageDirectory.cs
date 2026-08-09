namespace ImageNormalizer.ImageFileSystem;

public interface IImageDirectory
{
	ExitCode BuildImageDirectory();

	ExitCode NormalizeImages();
}
