namespace StbImageSharp;

/// <summary>
/// Specifies the color components present in image data.
/// </summary>
#if !STBSHARP_INTERNAL
public
#else
internal
#endif
enum ColorComponents
{
	/// <summary>
	/// Use the source image's native color components.
	/// </summary>
	Default,

	/// <summary>
	/// Single-channel greyscale.
	/// </summary>
	Grey,

	/// <summary>
	/// Two-channel greyscale with alpha.
	/// </summary>
	GreyAlpha,

	/// <summary>
	/// Three-channel red, green, blue.
	/// </summary>
	RedGreenBlue,

	/// <summary>
	/// Four-channel red, green, blue, alpha.
	/// </summary>
	RedGreenBlueAlpha
}
