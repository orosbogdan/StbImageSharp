using System.IO;

namespace StbImageSharp;

#if !STBSHARP_INTERNAL
public
#else
internal
#endif
readonly struct ImageInfo
{
	/// <summary>
	/// Gets the width of the image in pixels.
	/// </summary>
	public int Width { get; init; }

	/// <summary>
	/// Gets the height of the image in pixels.
	/// </summary>
	public int Height { get; init; }

	/// <summary>
	/// Gets the color components of the image.
	/// </summary>
	public ColorComponents ColorComponents { get; init; }

	/// <summary>
	/// Gets the number of bits per channel (8 or 16).
	/// </summary>
	public int BitsPerChannel { get; init; }

	/// <summary>
	/// Reads image metadata from a stream without decoding the full image.
	/// </summary>
	/// <param name="stream">The stream containing the image data.</param>
	/// <returns>An <see cref="ImageInfo"/> if the image format is recognized; otherwise, <see langword="null"/>.</returns>
	public static unsafe ImageInfo? FromStream(Stream stream)
	{
		int width, height, comp;
		var context = new StbImage.stbi__context(stream);

		var is16Bit = StbImage.stbi__is_16_main(context) == 1;
		StbImage.stbi__rewind(context);

		var infoResult = StbImage.stbi__info_main(context, &width, &height, &comp);
		StbImage.stbi__rewind(context);

		if (infoResult == 0) return null;

		return new ImageInfo
		{
			Width = width,
			Height = height,
			ColorComponents = (ColorComponents)comp,
			BitsPerChannel = is16Bit ? 16 : 8
		};
	}
}
