using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting;

public sealed class ShapedBuffer : IReadOnlyList<GlyphInfo>, IEnumerable<GlyphInfo>, IEnumerable, IReadOnlyCollection<GlyphInfo>, IDisposable
{
	private GlyphInfo[]? _rentedBuffer;

	private ArraySlice<GlyphInfo> _glyphInfos;

	public int Length => _glyphInfos.Length;

	public IGlyphTypeface GlyphTypeface { get; }

	public double FontRenderingEmSize { get; }

	public sbyte BidiLevel { get; }

	public bool IsLeftToRight => (BidiLevel & 1) == 0;

	public ReadOnlyMemory<char> Text { get; }

	public GlyphInfo this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _glyphInfos[index];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_glyphInfos[index] = value;
		}
	}

	int IReadOnlyCollection<GlyphInfo>.Count => _glyphInfos.Length;

	public ShapedBuffer(ReadOnlyMemory<char> text, int bufferLength, IGlyphTypeface glyphTypeface, double fontRenderingEmSize, sbyte bidiLevel)
	{
		Text = text;
		_rentedBuffer = ArrayPool<GlyphInfo>.Shared.Rent(bufferLength);
		_glyphInfos = new ArraySlice<GlyphInfo>(_rentedBuffer, 0, bufferLength);
		GlyphTypeface = glyphTypeface;
		FontRenderingEmSize = fontRenderingEmSize;
		BidiLevel = bidiLevel;
	}

	internal ShapedBuffer(ReadOnlyMemory<char> text, ArraySlice<GlyphInfo> glyphInfos, IGlyphTypeface glyphTypeface, double fontRenderingEmSize, sbyte bidiLevel)
	{
		Text = text;
		_glyphInfos = glyphInfos;
		GlyphTypeface = glyphTypeface;
		FontRenderingEmSize = fontRenderingEmSize;
		BidiLevel = bidiLevel;
	}

	public void Reverse()
	{
		_glyphInfos.Span.Reverse();
	}

	public void Dispose()
	{
		if (_rentedBuffer != null)
		{
			ArrayPool<GlyphInfo>.Shared.Return(_rentedBuffer);
			_rentedBuffer = null;
			_glyphInfos = ArraySlice<GlyphInfo>.Empty;
		}
	}

	public IEnumerator<GlyphInfo> GetEnumerator()
	{
		return _glyphInfos.GetEnumerator();
	}

	private int FindGlyphIndex(int characterIndex)
	{
		if (characterIndex < _glyphInfos[0].GlyphCluster)
		{
			return 0;
		}
		if (characterIndex > _glyphInfos[_glyphInfos.Length - 1].GlyphCluster)
		{
			return _glyphInfos.Length - 1;
		}
		Comparer<GlyphInfo> clusterAscendingComparer = GlyphInfo.ClusterAscendingComparer;
		Span<GlyphInfo> span = _glyphInfos.Span;
		GlyphInfo value = new GlyphInfo(0, characterIndex, 0.0);
		int num = span.BinarySearch(value, clusterAscendingComparer);
		if (num < 0)
		{
			while (characterIndex > 0 && num < 0)
			{
				characterIndex--;
				value = new GlyphInfo(0, characterIndex, 0.0);
				num = span.BinarySearch(value, clusterAscendingComparer);
			}
			if (num < 0)
			{
				return -1;
			}
		}
		while (num > 0 && span[num - 1].GlyphCluster == span[num].GlyphCluster)
		{
			num--;
		}
		return num;
	}

	internal SplitResult<ShapedBuffer> Split(int length)
	{
		if (Text.Length == length)
		{
			return new SplitResult<ShapedBuffer>(this, null);
		}
		int glyphCluster = _glyphInfos[0].GlyphCluster;
		int glyphCluster2 = _glyphInfos[_glyphInfos.Length - 1].GlyphCluster;
		int num = ((glyphCluster < glyphCluster2) ? glyphCluster : glyphCluster2);
		int length2 = FindGlyphIndex(num + length);
		ShapedBuffer first = new ShapedBuffer(Text.Slice(0, length), _glyphInfos.Take(length2), GlyphTypeface, FontRenderingEmSize, BidiLevel);
		ShapedBuffer second = new ShapedBuffer(Text.Slice(length), _glyphInfos.Skip(length2), GlyphTypeface, FontRenderingEmSize, BidiLevel);
		return new SplitResult<ShapedBuffer>(first, second);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
