using StbImageSharp.Hebron.Runtime;
using System;
using System.IO;

namespace StbImageSharp;

#if !STBSHARP_INTERNAL
public
#else
internal
#endif
static unsafe partial class StbImage
{
	[ThreadStatic]
	internal static string? stbi__g_failure_reason;

	/// <summary>
	/// Gets the current number of outstanding native memory allocations.
	/// </summary>
	public static int NativeAllocations => MemoryStats.Allocations;

	public class stbi__context
	{
		private readonly Stream _stream;

		public byte[]? _tempBuffer;
		public int img_n = 0;
		public int img_out_n = 0;
		public uint img_x = 0;
		public uint img_y = 0;

		public stbi__context(Stream stream)
		{
			ArgumentNullException.ThrowIfNull(stream);
			_stream = stream;
		}

		public Stream Stream => _stream;
	}

	internal static int stbi__err(string str)
	{
		stbi__g_failure_reason = str;
		return 0;
	}

	internal static byte stbi__get8(stbi__context s)
	{
		var b = s.Stream.ReadByte();
		if (b == -1) return 0;

		return (byte)b;
	}

	internal static void stbi__skip(stbi__context s, int skip)
	{
		s.Stream.Seek(skip, SeekOrigin.Current);
	}

	internal static void stbi__rewind(stbi__context s)
	{
		s.Stream.Seek(0, SeekOrigin.Begin);
	}

	internal static int stbi__at_eof(stbi__context s)
	{
		return s.Stream.Position == s.Stream.Length ? 1 : 0;
	}

	internal static int stbi__getn(stbi__context s, byte* buf, int size)
	{
		if (s._tempBuffer is null ||
			s._tempBuffer.Length < size)
			s._tempBuffer = new byte[size * 2];

		var result = s.Stream.Read(s._tempBuffer, 0, size);
		s._tempBuffer.AsSpan(0, result).CopyTo(new Span<byte>(buf, result));

		return result;
	}
}
