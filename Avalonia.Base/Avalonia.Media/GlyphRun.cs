using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Media.TextFormatting;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class GlyphRun : IDisposable
{
	private static readonly IPlatformRenderInterface s_renderInterface;

	private IRef<IGlyphRunImpl>? _platformImpl;

	private double _fontRenderingEmSize;

	private int _biDiLevel;

	private GlyphRunMetrics? _glyphRunMetrics;

	private ReadOnlyMemory<char> _characters;

	private IReadOnlyList<GlyphInfo> _glyphInfos;

	private Point? _baselineOrigin;

	private bool _hasOneCharPerCluster;

	public IGlyphTypeface GlyphTypeface { get; }

	public double FontRenderingEmSize
	{
		get
		{
			return _fontRenderingEmSize;
		}
		set
		{
			Set(ref _fontRenderingEmSize, value);
		}
	}

	public Rect Bounds => new Rect(new Size(Metrics.WidthIncludingTrailingWhitespace, Metrics.Height));

	public Rect InkBounds => PlatformImpl.Item.Bounds;

	public GlyphRunMetrics Metrics
	{
		get
		{
			GlyphRunMetrics valueOrDefault = _glyphRunMetrics.GetValueOrDefault();
			if (!_glyphRunMetrics.HasValue)
			{
				valueOrDefault = CreateGlyphRunMetrics();
				_glyphRunMetrics = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public Point BaselineOrigin
	{
		get
		{
			return _baselineOrigin ?? new Point(0.0, Metrics.Baseline);
		}
		set
		{
			Set(ref _baselineOrigin, value);
		}
	}

	public ReadOnlyMemory<char> Characters
	{
		get
		{
			return _characters;
		}
		set
		{
			Set(ref _characters, value);
		}
	}

	public IReadOnlyList<GlyphInfo> GlyphInfos
	{
		get
		{
			return _glyphInfos;
		}
		set
		{
			Set(ref _glyphInfos, value);
			_hasOneCharPerCluster = false;
		}
	}

	public int BiDiLevel
	{
		get
		{
			return _biDiLevel;
		}
		set
		{
			Set(ref _biDiLevel, value);
		}
	}

	internal double Scale => FontRenderingEmSize / (double)GlyphTypeface.Metrics.DesignEmHeight;

	public bool IsLeftToRight => (BiDiLevel & 1) == 0;

	internal IRef<IGlyphRunImpl> PlatformImpl => _platformImpl ?? (_platformImpl = CreateGlyphRunImpl());

	static GlyphRun()
	{
		s_renderInterface = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
	}

	public GlyphRun(IGlyphTypeface glyphTypeface, double fontRenderingEmSize, ReadOnlyMemory<char> characters, IReadOnlyList<ushort> glyphIndices, Point? baselineOrigin = null, int biDiLevel = 0)
		: this(glyphTypeface, fontRenderingEmSize, characters, CreateGlyphInfos(glyphIndices, fontRenderingEmSize, glyphTypeface), baselineOrigin, biDiLevel)
	{
		_hasOneCharPerCluster = true;
	}

	public GlyphRun(IGlyphTypeface glyphTypeface, double fontRenderingEmSize, ReadOnlyMemory<char> characters, IReadOnlyList<GlyphInfo> glyphInfos, Point? baselineOrigin = null, int biDiLevel = 0)
	{
		GlyphTypeface = glyphTypeface;
		_fontRenderingEmSize = fontRenderingEmSize;
		_characters = characters;
		_glyphInfos = glyphInfos;
		_baselineOrigin = baselineOrigin;
		_biDiLevel = biDiLevel;
	}

	internal GlyphRun(IRef<IGlyphRunImpl> platformImpl)
	{
		_glyphInfos = Array.Empty<GlyphInfo>();
		GlyphTypeface = Typeface.Default.GlyphTypeface;
		_platformImpl = platformImpl;
		_baselineOrigin = platformImpl.Item.BaselineOrigin;
	}

	private static IReadOnlyList<GlyphInfo> CreateGlyphInfos(IReadOnlyList<ushort> glyphIndices, double fontRenderingEmSize, IGlyphTypeface glyphTypeface)
	{
		ReadOnlySpan<ushort> glyphs = ListToSpan(glyphIndices);
		int[] glyphAdvances = glyphTypeface.GetGlyphAdvances(glyphs);
		GlyphInfo[] array = new GlyphInfo[glyphs.Length];
		double num = fontRenderingEmSize / (double)glyphTypeface.Metrics.DesignEmHeight;
		for (int i = 0; i < glyphs.Length; i++)
		{
			array[i] = new GlyphInfo(glyphs[i], i, (double)glyphAdvances[i] * num);
		}
		return array;
	}

	private static ReadOnlySpan<ushort> ListToSpan(IReadOnlyList<ushort> list)
	{
		int count = list.Count;
		if (count == 0)
		{
			return default(ReadOnlySpan<ushort>);
		}
		if (list is ushort[] array)
		{
			return array.AsSpan();
		}
		if (list is List<ushort> list2)
		{
			return CollectionsMarshal.AsSpan(list2);
		}
		ushort[] array2 = new ushort[count];
		for (int i = 0; i < count; i++)
		{
			array2[i] = list[i];
		}
		return array2.AsSpan();
	}

	public Geometry BuildGeometry()
	{
		return new PlatformGeometry(s_renderInterface.BuildGlyphRunGeometry(this));
	}

	public double GetDistanceFromCharacterHit(CharacterHit characterHit)
	{
		int num = characterHit.FirstCharacterIndex + characterHit.TrailingLength;
		double num2 = 0.0;
		if (IsLeftToRight)
		{
			if (num < Metrics.FirstCluster)
			{
				return 0.0;
			}
			if (num > Metrics.LastCluster)
			{
				return Bounds.Width;
			}
			int i = FindGlyphIndex(num);
			int glyphCluster = _glyphInfos[i].GlyphCluster;
			if (characterHit.TrailingLength > 0)
			{
				for (; i + 1 < _glyphInfos.Count && _glyphInfos[i + 1].GlyphCluster == glyphCluster; i++)
				{
				}
			}
			for (int j = 0; j < i; j++)
			{
				num2 += _glyphInfos[j].GlyphAdvance;
			}
			return num2;
		}
		int num3 = FindGlyphIndex(num);
		if (num > Metrics.LastCluster)
		{
			return 0.0;
		}
		if (num <= Metrics.FirstCluster)
		{
			return Bounds.Width;
		}
		for (int k = num3 + 1; k < _glyphInfos.Count; k++)
		{
			num2 += _glyphInfos[k].GlyphAdvance;
		}
		return Bounds.Width - num2;
	}

	public CharacterHit GetCharacterHitFromDistance(double distance, out bool isInside)
	{
		double width;
		if (distance <= 0.0)
		{
			isInside = false;
			CharacterHit result = FindNearestCharacterHit(IsLeftToRight ? Metrics.FirstCluster : Metrics.LastCluster, out width);
			if (!IsLeftToRight)
			{
				return result;
			}
			return new CharacterHit(result.FirstCharacterIndex);
		}
		if (distance >= Bounds.Width)
		{
			isInside = false;
			CharacterHit result2 = FindNearestCharacterHit(IsLeftToRight ? Metrics.LastCluster : Metrics.FirstCluster, out width);
			if (!IsLeftToRight)
			{
				return new CharacterHit(result2.FirstCharacterIndex);
			}
			return result2;
		}
		int index = 0;
		double num = 0.0;
		if (IsLeftToRight)
		{
			for (int i = 0; i < _glyphInfos.Count; i++)
			{
				GlyphInfo glyphInfo = _glyphInfos[i];
				double glyphAdvance = glyphInfo.GlyphAdvance;
				index = glyphInfo.GlyphCluster;
				if (distance > num && distance <= num + glyphAdvance)
				{
					break;
				}
				num += glyphAdvance;
			}
		}
		else
		{
			num = Bounds.Width;
			for (int num2 = _glyphInfos.Count - 1; num2 >= 0; num2--)
			{
				GlyphInfo glyphInfo2 = _glyphInfos[num2];
				double glyphAdvance2 = glyphInfo2.GlyphAdvance;
				index = glyphInfo2.GlyphCluster;
				if (num - glyphAdvance2 < distance)
				{
					break;
				}
				num -= glyphAdvance2;
			}
		}
		isInside = true;
		double width2;
		CharacterHit result3 = FindNearestCharacterHit(index, out width2);
		double num3 = width2 / 2.0;
		if (!((IsLeftToRight ? Math.Round(distance - num, 3) : Math.Round(num - distance, 3)) > num3))
		{
			return new CharacterHit(result3.FirstCharacterIndex);
		}
		return result3;
	}

	public CharacterHit GetNextCaretCharacterHit(CharacterHit characterHit)
	{
		double width;
		if (characterHit.TrailingLength == 0)
		{
			characterHit = FindNearestCharacterHit(characterHit.FirstCharacterIndex, out width);
			if (characterHit.FirstCharacterIndex == Metrics.LastCluster)
			{
				return characterHit;
			}
			return new CharacterHit(characterHit.FirstCharacterIndex + characterHit.TrailingLength);
		}
		return FindNearestCharacterHit(characterHit.FirstCharacterIndex + characterHit.TrailingLength, out width);
	}

	public CharacterHit GetPreviousCaretCharacterHit(CharacterHit characterHit)
	{
		double width;
		CharacterHit result = FindNearestCharacterHit(characterHit.FirstCharacterIndex - 1, out width);
		if (characterHit.TrailingLength != 0)
		{
			return result;
		}
		return new CharacterHit(result.FirstCharacterIndex);
	}

	public int FindGlyphIndex(int characterIndex)
	{
		if (_hasOneCharPerCluster)
		{
			return characterIndex;
		}
		if (characterIndex > Metrics.LastCluster)
		{
			if (IsLeftToRight)
			{
				return _glyphInfos.Count - 1;
			}
			return 0;
		}
		if (characterIndex < Metrics.FirstCluster)
		{
			if (IsLeftToRight)
			{
				return 0;
			}
			return _glyphInfos.Count - 1;
		}
		Comparer<GlyphInfo> comparer = (IsLeftToRight ? GlyphInfo.ClusterAscendingComparer : GlyphInfo.ClusterDescendingComparer);
		int i = _glyphInfos.BinarySearch(new GlyphInfo(0, characterIndex, 0.0), comparer);
		if (i < 0)
		{
			while (characterIndex > 0 && i < 0)
			{
				characterIndex--;
				i = _glyphInfos.BinarySearch(new GlyphInfo(0, characterIndex, 0.0), comparer);
			}
			if (i < 0)
			{
				return 0;
			}
		}
		if (IsLeftToRight)
		{
			while (i > 0 && _glyphInfos[i - 1].GlyphCluster == _glyphInfos[i].GlyphCluster)
			{
				i--;
			}
		}
		else
		{
			for (; i + 1 < _glyphInfos.Count && _glyphInfos[i + 1].GlyphCluster == _glyphInfos[i].GlyphCluster; i++)
			{
			}
		}
		if (i < 0)
		{
			return 0;
		}
		if (i > _glyphInfos.Count - 1)
		{
			return _glyphInfos.Count - 1;
		}
		return i;
	}

	public CharacterHit FindNearestCharacterHit(int index, out double width)
	{
		width = 0.0;
		int num = FindGlyphIndex(index);
		if (_hasOneCharPerCluster)
		{
			width = _glyphInfos[index].GlyphAdvance;
			return new CharacterHit(num, 1);
		}
		int glyphCluster = _glyphInfos[num].GlyphCluster;
		int num2 = glyphCluster;
		int num3 = num;
		while (num2 == glyphCluster)
		{
			width += _glyphInfos[num3].GlyphAdvance;
			if (IsLeftToRight)
			{
				num3++;
				if (num3 == _glyphInfos.Count)
				{
					break;
				}
			}
			else
			{
				num3--;
				if (num3 < 0)
				{
					break;
				}
			}
			num2 = _glyphInfos[num3].GlyphCluster;
		}
		int num4 = Math.Max(0, num2 - glyphCluster);
		if (glyphCluster == Metrics.LastCluster && num4 == 0)
		{
			int num5 = 0;
			int num6 = Metrics.FirstCluster;
			if (IsLeftToRight)
			{
				for (int i = 1; i < _glyphInfos.Count; i++)
				{
					num2 = _glyphInfos[i].GlyphCluster;
					if (num6 > glyphCluster)
					{
						break;
					}
					int num7 = num2 - num6;
					num5 += num7;
					num6 = num2;
				}
			}
			else
			{
				for (int num8 = _glyphInfos.Count - 1; num8 >= 0; num8--)
				{
					num2 = _glyphInfos[num8].GlyphCluster;
					if (num6 > glyphCluster)
					{
						break;
					}
					int num9 = num2 - num6;
					num5 += num9;
					num6 = num2;
				}
			}
			num4 = (Characters.IsEmpty ? 1 : (Characters.Length - num5));
		}
		return new CharacterHit(glyphCluster, num4);
	}

	private GlyphRunMetrics CreateGlyphRunMetrics()
	{
		int num;
		int num2;
		if (Characters.IsEmpty)
		{
			num = 0;
			num2 = 0;
		}
		else
		{
			num = _glyphInfos[0].GlyphCluster;
			num2 = _glyphInfos[_glyphInfos.Count - 1].GlyphCluster;
		}
		bool flag = num > num2;
		if (!IsLeftToRight)
		{
			int num3 = num;
			int num4 = num2;
			num2 = num3;
			num = num4;
		}
		double height = (double)GlyphTypeface.Metrics.LineSpacing * Scale;
		double num5 = 0.0;
		int newLineLength;
		int glyphCount;
		int trailingWhitespaceLength = GetTrailingWhitespaceLength(flag, out newLineLength, out glyphCount);
		for (int i = 0; i < _glyphInfos.Count; i++)
		{
			double glyphAdvance = _glyphInfos[i].GlyphAdvance;
			num5 += glyphAdvance;
		}
		double num6 = num5;
		if (flag)
		{
			for (int j = 0; j < glyphCount; j++)
			{
				num6 -= _glyphInfos[j].GlyphAdvance;
			}
		}
		else
		{
			for (int k = _glyphInfos.Count - glyphCount; k < _glyphInfos.Count; k++)
			{
				num6 -= _glyphInfos[k].GlyphAdvance;
			}
		}
		GlyphRunMetrics result = default(GlyphRunMetrics);
		result.Baseline = (double)(-GlyphTypeface.Metrics.Ascent) * Scale;
		result.Width = num6;
		result.WidthIncludingTrailingWhitespace = num5;
		result.Height = height;
		result.NewLineLength = newLineLength;
		result.TrailingWhitespaceLength = trailingWhitespaceLength;
		result.FirstCluster = num;
		result.LastCluster = num2;
		return result;
	}

	private int GetTrailingWhitespaceLength(bool isReversed, out int newLineLength, out int glyphCount)
	{
		if (isReversed)
		{
			return GetTrailingWhitespaceLengthRightToLeft(out newLineLength, out glyphCount);
		}
		glyphCount = 0;
		newLineLength = 0;
		int num = 0;
		ReadOnlySpan<char> span = _characters.Span;
		if (!span.IsEmpty)
		{
			int num2 = span.Length - 1;
			for (int num3 = _glyphInfos.Count - 1; num3 >= 0; num3--)
			{
				int glyphCluster = _glyphInfos[num3].GlyphCluster;
				int count;
				Codepoint codepoint = Codepoint.ReadAt(span, num2, out count);
				num2 -= count;
				if (!codepoint.IsWhiteSpace)
				{
					break;
				}
				int num4 = 1;
				while (num3 - 1 >= 0)
				{
					int glyphCluster2 = _glyphInfos[num3 - 1].GlyphCluster;
					if (glyphCluster != glyphCluster2)
					{
						break;
					}
					num4++;
					num3--;
					if (num2 >= 0)
					{
						codepoint = Codepoint.ReadAt(span, num2, out count);
						num2 -= count;
					}
				}
				if (codepoint.IsBreakChar)
				{
					newLineLength += num4;
				}
				num += num4;
				glyphCount++;
			}
		}
		return num;
	}

	private int GetTrailingWhitespaceLengthRightToLeft(out int newLineLength, out int glyphCount)
	{
		glyphCount = 0;
		newLineLength = 0;
		int num = 0;
		ReadOnlySpan<char> span = Characters.Span;
		if (!span.IsEmpty)
		{
			int num2 = span.Length - 1;
			for (int i = 0; i < _glyphInfos.Count; i++)
			{
				int glyphCluster = _glyphInfos[i].GlyphCluster;
				int count;
				Codepoint codepoint = Codepoint.ReadAt(span, num2, out count);
				if (!codepoint.IsWhiteSpace)
				{
					break;
				}
				int num3 = 1;
				int num4 = i;
				while (num4 + 1 < _glyphInfos.Count)
				{
					int glyphCluster2 = _glyphInfos[++num4].GlyphCluster;
					if (glyphCluster != glyphCluster2)
					{
						break;
					}
					num3++;
				}
				num2 -= num3;
				if (codepoint.IsBreakChar)
				{
					newLineLength += num3;
				}
				num += num3;
				glyphCount += num3;
			}
		}
		return num;
	}

	private void Set<T>(ref T field, T value)
	{
		_platformImpl?.Dispose();
		_platformImpl = null;
		_glyphRunMetrics = null;
		field = value;
	}

	private IRef<IGlyphRunImpl> CreateGlyphRunImpl()
	{
		IGlyphRunImpl item = s_renderInterface.CreateGlyphRun(GlyphTypeface, FontRenderingEmSize, GlyphInfos, BaselineOrigin);
		_platformImpl = RefCountable.Create(item);
		return _platformImpl;
	}

	public void Dispose()
	{
		_platformImpl?.Dispose();
		_platformImpl = null;
	}

	public IReadOnlyList<float> GetIntersections(float lowerLimit, float upperLimit)
	{
		return PlatformImpl.Item.GetIntersections(lowerLimit, upperLimit);
	}

	public IImmutableGlyphRunReference? TryCreateImmutableGlyphRunReference()
	{
		return new ImmutableGlyphRunReference(PlatformImpl.Clone());
	}
}
