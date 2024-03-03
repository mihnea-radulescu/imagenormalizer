# imagenormalizer
Image Normalizer is a cross-platform command-line batch-processing tool that resizes and compresses images, transforming each image by:
* removing its CICP, EXIF, ICC, IPTC and XMP profile data, if present
* resizing it to fit within the maximum width/height supplied, if necessary
* compressing it to the provided image quality level
* saving it in the JPEG format with the .jpg file extension

Image Normalizer is intended to support the storage of images produced by digital cameras in a consistent and disc space conserving manner.

Image Normalizer targets .NET 8 on Linux, Windows and macOS.

__Usage__: ImageNormalizer [--max-width-height] [--quality] [--max-degree-of-parallelism] [--help] [--version] input-directory output-directory

Arguments:
* 0: input-directory - The input directory (Required)
* 1: output-directory - The output directory (Required)

Options:
* -m, --max-width-height - The output maximum image width/height (Default: 3840)
* -q, --quality - The output image quality (Default: 80)
* -p, --max-degree-of-parallelism - The maximum degree of parallel image processing, upper-bounded by processor count (Default: 16)
* -h, --help - Show help message
* --version - Show version
