namespace StbImageSharp;

#if !STBSHARP_INTERNAL
public
#else
internal
#endif
sealed class AnimatedFrameResult : ImageResult
{
	/// <summary>
	/// Gets the delay for this frame in milliseconds.
	/// </summary>
	public int DelayInMs { get; internal set; }
}
