# imagenormalizer
Image Normalizer is a minimalist command-line batch-processing tool that compresses images, transforming each image by:
* removing its CICP, EXIF, ICC, IPTC and XMP profile data, if present
* compressing it to the provided image quality level
* saving it in the JPEG format with the .jpg file extension

Image Normalizer is intended to support the storage of images produced by digital cameras in a consistent and disc space conserving manner.

Image Normalizer targets .NET 8 on Windows, Linux and macOS.

__Usage__: ImageNormalizer _[InputDirectory]_ _[OutputDirectory]_ _[OutputImageQuality]_
* _InputDirectory_: Required - The input directory containing the images to be processed (recursive processing).
* _OutputDirectory_: Required - The output directory to place the processed images into.
* _OutputImageQuality_: Optional - The output image quality, as an integer between 1 and 100 (default is 80).
