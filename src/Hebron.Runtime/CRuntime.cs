using System.Runtime.InteropServices;
 
namespace StbImageSharp.Hebron.Runtime;

internal static unsafe class CRuntime
{
	private static readonly string numbers = "0123456789";

	public static void* malloc(ulong size) => malloc((long)size);

	public static void* malloc(long size)
	{
		var ptr = NativeMemory.Alloc((nuint)size);
		MemoryStats.Allocated();
		return ptr;
	}

	public static void free(void* a)
	{
		if (a == null)
			return;

		NativeMemory.Free(a);
		MemoryStats.Freed();
	}

	public static void memcpy(void* a, void* b, long size)
	{
		Buffer.MemoryCopy(b, a, size, size);
	}

	public static void memcpy(void* a, void* b, ulong size) => memcpy(a, b, (long)size);

	public static void memmove(void* a, void* b, long size)
	{
		Buffer.MemoryCopy(b, a, size, size);
	}

	public static int memcmp(void* a, void* b, long size)
	{
		return new ReadOnlySpan<byte>(a, (int)size).SequenceCompareTo(
			new ReadOnlySpan<byte>(b, (int)size));
	}

	public static void memset(void* ptr, int value, long size)
	{
		NativeMemory.Fill(ptr, (nuint)size, (byte)value);
	}

	public static void memset(void* ptr, int value, ulong size) => memset(ptr, value, (long)size);

	public static uint _lrotl(uint x, int y) => (x << y) | (x >> (32 - y));

	public static void* realloc(void* a, long newSize)
	{
		if (a == null)
			return malloc(newSize);

		return NativeMemory.Realloc(a, (nuint)newSize);
	}

	public static void* realloc(void* a, ulong newSize) => realloc(a, (long)newSize);

	public static int abs(int v) => Math.Abs(v);

	public static double pow(double a, double b) => Math.Pow(a, b);

	public static double ldexp(double number, int exponent) => number * Math.Pow(2, exponent);

	public static int strcmp(sbyte* src, string token)
	{
		for (var i = 0; i < token.Length; ++i)
		{
			if (src[i] != token[i])
				return src[i] - token[i];
		}

		return src[token.Length];
	}

	public static int strncmp(sbyte* src, string token, ulong size)
	{
		var len = (int)Math.Min((ulong)token.Length, size);
		for (var i = 0; i < len; ++i)
		{
			if (src[i] != token[i])
				return src[i] - token[i];
		}

		return 0;
	}

	public static long strtol(sbyte* start, sbyte** end, int radix)
	{
		var length = 0;
		sbyte* ptr = start;
		while (numbers.IndexOf((char)*ptr) != -1)
		{
			++ptr;
			++length;
		}

		long result = 0;

		ptr = start;
		while (length > 0)
		{
			long num = numbers.IndexOf((char)*ptr);
			long pow = (long)Math.Pow(10, length - 1);
			result += num * pow;

			++ptr;
			--length;
		}

		if (end != null)
		{
			*end = ptr;
		}

		return result;
	}
}
