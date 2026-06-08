using System.IO;
using System.Runtime.InteropServices;
using StbImageSharp.Hebron.Runtime;

namespace StbImageSharp;

#if !STBSHARP_INTERNAL
public
#else
internal
#endif
sealed class ImageResultFloat
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
	/// Gets the raw floating-point pixel data of the decoded HDR image.
	/// </summary>
	public float[] Data { get; internal set; } = [];

	internal static unsafe ImageResultFloat FromResult(float* result, int width, int height, ColorComponents comp,
		ColorComponents req_comp)
	{
		if (result == null)
			throw new InvalidOperationException(StbImage.stbi__g_failure_reason);

		var image = new ImageResultFloat
		{
			Width = width,
			Height = height,
			SourceComp = comp,
			Comp = req_comp == ColorComponents.Default ? comp : req_comp
		};

		image.Data = new float[width * height * (int)image.Comp];
		new ReadOnlySpan<float>(result, image.Data.Length).CopyTo(image.Data);

		return image;
	}

	/// <summary>
	/// Loads an HDR image from a stream as floating-point data.
	/// </summary>
	/// <param name="stream">The stream containing the HDR image data.</param>
	/// <param name="requiredComponents">The desired color components for the output. Use <see cref="ColorComponents.Default"/> to preserve the source format.</param>
	/// <returns>An <see cref="ImageResultFloat"/> containing the decoded HDR image data.</returns>
	public static unsafe ImageResultFloat FromStream(Stream stream,
		ColorComponents requiredComponents = ColorComponents.Default)
	{
		float* result = null;

		try
		{
			int x, y, comp;

			var context = new StbImage.stbi__context(stream);

			result = StbImage.stbi__loadf_main(context, &x, &y, &comp, (int)requiredComponents);

			return FromResult(result, x, y, (ColorComponents)comp, requiredComponents);
		}
		finally
		{
			if (result != null)
				CRuntime.free(result);
		}
	}

	/// <summary>
	/// Loads an HDR image from a byte array as floating-point data.
	/// </summary>
	/// <param name="data">The byte array containing the HDR image data.</param>
	/// <param name="requiredComponents">The desired color components for the output. Use <see cref="ColorComponents.Default"/> to preserve the source format.</param>
	/// <returns>An <see cref="ImageResultFloat"/> containing the decoded HDR image data.</returns>
	public static ImageResultFloat FromMemory(byte[] data,
		ColorComponents requiredComponents = ColorComponents.Default)
	{
		using var stream = new MemoryStream(data);
		return FromStream(stream, requiredComponents);
	}
}
