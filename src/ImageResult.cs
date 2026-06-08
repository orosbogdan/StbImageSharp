using System.Collections.Generic;
using System.IO;
using StbImageSharp.Hebron.Runtime;
using System.Runtime.InteropServices;

namespace StbImageSharp;

#if !STBSHARP_INTERNAL
public
#else
internal
#endif
class ImageResult
{
	/// <summary>
	/// Gets the width of the image in pixels.
	/// </summary>
	public int Width { get; internal set; }

	/// <summary>
	/// Gets the height of the image in pixels.
	/// </summary>
	public int Height { get; internal set; }

	/// <summary>
	/// Gets the original color components of the source image.
	/// </summary>
	public ColorComponents SourceComp { get; internal set; }

	/// <summary>
	/// Gets the color components of the decoded image data.
	/// </summary>
	public ColorComponents Comp { get; internal set; }

	/// <summary>
	/// Gets the raw pixel data of the decoded image.
	/// </summary>
	public byte[] Data { get; internal set; } = [];

	internal static unsafe ImageResult FromResult(byte* result, int width, int height, ColorComponents comp,
		ColorComponents req_comp)
	{
		if (result == null)
			throw new InvalidOperationException(StbImage.stbi__g_failure_reason);

		var image = new ImageResult
		{
			Width = width,
			Height = height,
			SourceComp = comp,
			Comp = req_comp == ColorComponents.Default ? comp : req_comp
		};

		image.Data = new byte[width * height * (int)image.Comp];
		new ReadOnlySpan<byte>(result, image.Data.Length).CopyTo(image.Data);

		return image;
	}

	/// <summary>
	/// Loads an image from a stream.
	/// </summary>
	/// <param name="stream">The stream containing the image data.</param>
	/// <param name="requiredComponents">The desired color components for the output. Use <see cref="ColorComponents.Default"/> to preserve the source format.</param>
	/// <returns>An <see cref="ImageResult"/> containing the decoded image data.</returns>
	public static unsafe ImageResult FromStream(Stream stream,
		ColorComponents requiredComponents = ColorComponents.Default)
	{
		byte* result = null;

		try
		{
			int x, y, comp;

			var context = new StbImage.stbi__context(stream);

			result = StbImage.stbi__load_and_postprocess_8bit(context, &x, &y, &comp, (int)requiredComponents);

			return FromResult(result, x, y, (ColorComponents)comp, requiredComponents);
		}
		finally
		{
			if (result != null)
				CRuntime.free(result);
		}
	}

	/// <summary>
	/// Loads an image from a byte array.
	/// </summary>
	/// <param name="data">The byte array containing the image data.</param>
	/// <param name="requiredComponents">The desired color components for the output. Use <see cref="ColorComponents.Default"/> to preserve the source format.</param>
	/// <returns>An <see cref="ImageResult"/> containing the decoded image data.</returns>
	public static ImageResult FromMemory(byte[] data, ColorComponents requiredComponents = ColorComponents.Default)
	{
		using var stream = new MemoryStream(data);
		return FromStream(stream, requiredComponents);
	}

	/// <summary>
	/// Returns an enumerable sequence of animated GIF frames from a stream.
	/// </summary>
	/// <param name="stream">The stream containing the animated GIF data.</param>
	/// <param name="requiredComponents">The desired color components for each frame.</param>
	/// <returns>An enumerable of <see cref="AnimatedFrameResult"/> representing each frame.</returns>
	public static IEnumerable<AnimatedFrameResult> AnimatedGifFramesFromStream(Stream stream,
		ColorComponents requiredComponents = ColorComponents.Default)
	{
		return new AnimatedGifEnumerable(stream, requiredComponents);
	}
}
