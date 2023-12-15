# imagenormalizer
Image Normalizer is a minimalist command-line batch-processing tool that normalizes images, by performing the following tranformations to each image:
* removes its EXIF data
* applies a consistent image compression level, based on the specified quality level
* saves it in the JPEG format

Image Normalizer is intended to support the storage of images produced by digital cameras in a consistent and disc space conserving manner.

Image Normalizer targets .NET 8 on Windows, Linux and macOS (requires .NET Runtime 8).

__Usage__: ImageNormalizer _[InputDirectory]_ _[OutputDirectory]_ _[OutputImageQuality]_
* _InputDirectory_: Required - The input directory containing the images to be processed (recursive processing).
* _OutputDirectory_: Required - The output directory to place the processed images into.
* _OutputImageQuality_: Optional - The output image quality, as an integer between 1 and 100 (default is 75).
