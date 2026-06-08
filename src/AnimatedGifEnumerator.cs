using StbImageSharp.Hebron.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace StbImageSharp;

internal sealed class AnimatedGifEnumerator : IEnumerator<AnimatedFrameResult>
{
	private readonly StbImage.stbi__context _context;
	private readonly ColorComponents _colorComponents;
	private StbImage.stbi__gif? _gif;
	private AnimatedFrameResult? _current;

	public AnimatedGifEnumerator(Stream input, ColorComponents colorComponents)
	{
		ArgumentNullException.ThrowIfNull(input);

		_context = new StbImage.stbi__context(input);

		if (StbImage.stbi__gif_test(_context) == 0)
			throw new InvalidOperationException("Input stream is not GIF file.");

		_gif = new StbImage.stbi__gif();
		_colorComponents = colorComponents;
	}

	public ColorComponents ColorComponents => _colorComponents;

	public AnimatedFrameResult Current =>
		_current ?? throw new InvalidOperationException("Enumeration has not started.");

	object IEnumerator.Current => Current;

	public void Dispose()
	{
		DisposeCore();
		GC.SuppressFinalize(this);
	}

	public unsafe bool MoveNext()
	{
		var gif = _gif ?? throw new ObjectDisposedException(nameof(AnimatedGifEnumerator));

		int ccomp;
		byte two_back;
		var result = StbImage.stbi__gif_load_next(_context, gif, &ccomp, (int)ColorComponents, &two_back);
		if (result == null) return false;

		if (_current is null)
		{
			var comp = ColorComponents == ColorComponents.Default
				? (ColorComponents)ccomp
				: ColorComponents;

			_current = new AnimatedFrameResult
			{
				Width = gif.w,
				Height = gif.h,
				SourceComp = (ColorComponents)ccomp,
				Comp = comp,
				Data = new byte[gif.w * gif.h * (int)comp]
			};
		}

		_current.DelayInMs = gif.delay;

		new ReadOnlySpan<byte>(result, _current.Data.Length).CopyTo(_current.Data);

		return true;
	}

	public void Reset() => throw new NotSupportedException();

	~AnimatedGifEnumerator()
	{
		DisposeCore();
	}

	private unsafe void DisposeCore()
	{
		if (_gif is null)
			return;

		if (_gif._out_ != null)
		{
			CRuntime.free(_gif._out_);
			_gif._out_ = null;
		}

		if (_gif.history != null)
		{
			CRuntime.free(_gif.history);
			_gif.history = null;
		}

		if (_gif.background != null)
		{
			CRuntime.free(_gif.background);
			_gif.background = null;
		}

		_gif = null;
	}
}

internal sealed class AnimatedGifEnumerable(Stream input, ColorComponents colorComponents) : IEnumerable<AnimatedFrameResult>
{
	public ColorComponents ColorComponents => colorComponents;

	public IEnumerator<AnimatedFrameResult> GetEnumerator() => new AnimatedGifEnumerator(input, ColorComponents);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
