namespace ImageNormalizer.Services;

public interface IImageNormalizerService
{
    void NormalizeImage(string inputFilePath, string outputFilePath);
}
