using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp;

public readonly struct Color : IEquatable<Color>
{
	private readonly Rgba64 data;

	private readonly IPixel? boxedHighPrecisionPixel;

	private static readonly Lazy<Dictionary<string, Color>> NamedColorsLookupLazy = new Lazy<Dictionary<string, Color>>(CreateNamedColorsLookup, isThreadSafe: true);

	public static readonly Color AliceBlue = FromRgba(240, 248, byte.MaxValue, byte.MaxValue);

	public static readonly Color AntiqueWhite = FromRgba(250, 235, 215, byte.MaxValue);

	public static readonly Color Aqua = FromRgba(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color Aquamarine = FromRgba(127, byte.MaxValue, 212, byte.MaxValue);

	public static readonly Color Azure = FromRgba(240, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color Beige = FromRgba(245, 245, 220, byte.MaxValue);

	public static readonly Color Bisque = FromRgba(byte.MaxValue, 228, 196, byte.MaxValue);

	public static readonly Color Black = FromRgba(0, 0, 0, byte.MaxValue);

	public static readonly Color BlanchedAlmond = FromRgba(byte.MaxValue, 235, 205, byte.MaxValue);

	public static readonly Color Blue = FromRgba(0, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color BlueViolet = FromRgba(138, 43, 226, byte.MaxValue);

	public static readonly Color Brown = FromRgba(165, 42, 42, byte.MaxValue);

	public static readonly Color BurlyWood = FromRgba(222, 184, 135, byte.MaxValue);

	public static readonly Color CadetBlue = FromRgba(95, 158, 160, byte.MaxValue);

	public static readonly Color Chartreuse = FromRgba(127, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color Chocolate = FromRgba(210, 105, 30, byte.MaxValue);

	public static readonly Color Coral = FromRgba(byte.MaxValue, 127, 80, byte.MaxValue);

	public static readonly Color CornflowerBlue = FromRgba(100, 149, 237, byte.MaxValue);

	public static readonly Color Cornsilk = FromRgba(byte.MaxValue, 248, 220, byte.MaxValue);

	public static readonly Color Crimson = FromRgba(220, 20, 60, byte.MaxValue);

	public static readonly Color Cyan = Aqua;

	public static readonly Color DarkBlue = FromRgba(0, 0, 139, byte.MaxValue);

	public static readonly Color DarkCyan = FromRgba(0, 139, 139, byte.MaxValue);

	public static readonly Color DarkGoldenrod = FromRgba(184, 134, 11, byte.MaxValue);

	public static readonly Color DarkGray = FromRgba(169, 169, 169, byte.MaxValue);

	public static readonly Color DarkGreen = FromRgba(0, 100, 0, byte.MaxValue);

	public static readonly Color DarkGrey = DarkGray;

	public static readonly Color DarkKhaki = FromRgba(189, 183, 107, byte.MaxValue);

	public static readonly Color DarkMagenta = FromRgba(139, 0, 139, byte.MaxValue);

	public static readonly Color DarkOliveGreen = FromRgba(85, 107, 47, byte.MaxValue);

	public static readonly Color DarkOrange = FromRgba(byte.MaxValue, 140, 0, byte.MaxValue);

	public static readonly Color DarkOrchid = FromRgba(153, 50, 204, byte.MaxValue);

	public static readonly Color DarkRed = FromRgba(139, 0, 0, byte.MaxValue);

	public static readonly Color DarkSalmon = FromRgba(233, 150, 122, byte.MaxValue);

	public static readonly Color DarkSeaGreen = FromRgba(143, 188, 143, byte.MaxValue);

	public static readonly Color DarkSlateBlue = FromRgba(72, 61, 139, byte.MaxValue);

	public static readonly Color DarkSlateGray = FromRgba(47, 79, 79, byte.MaxValue);

	public static readonly Color DarkSlateGrey = DarkSlateGray;

	public static readonly Color DarkTurquoise = FromRgba(0, 206, 209, byte.MaxValue);

	public static readonly Color DarkViolet = FromRgba(148, 0, 211, byte.MaxValue);

	public static readonly Color DeepPink = FromRgba(byte.MaxValue, 20, 147, byte.MaxValue);

	public static readonly Color DeepSkyBlue = FromRgba(0, 191, byte.MaxValue, byte.MaxValue);

	public static readonly Color DimGray = FromRgba(105, 105, 105, byte.MaxValue);

	public static readonly Color DimGrey = DimGray;

	public static readonly Color DodgerBlue = FromRgba(30, 144, byte.MaxValue, byte.MaxValue);

	public static readonly Color Firebrick = FromRgba(178, 34, 34, byte.MaxValue);

	public static readonly Color FloralWhite = FromRgba(byte.MaxValue, 250, 240, byte.MaxValue);

	public static readonly Color ForestGreen = FromRgba(34, 139, 34, byte.MaxValue);

	public static readonly Color Fuchsia = FromRgba(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color Gainsboro = FromRgba(220, 220, 220, byte.MaxValue);

	public static readonly Color GhostWhite = FromRgba(248, 248, byte.MaxValue, byte.MaxValue);

	public static readonly Color Gold = FromRgba(byte.MaxValue, 215, 0, byte.MaxValue);

	public static readonly Color Goldenrod = FromRgba(218, 165, 32, byte.MaxValue);

	public static readonly Color Gray = FromRgba(128, 128, 128, byte.MaxValue);

	public static readonly Color Green = FromRgba(0, 128, 0, byte.MaxValue);

	public static readonly Color GreenYellow = FromRgba(173, byte.MaxValue, 47, byte.MaxValue);

	public static readonly Color Grey = Gray;

	public static readonly Color Honeydew = FromRgba(240, byte.MaxValue, 240, byte.MaxValue);

	public static readonly Color HotPink = FromRgba(byte.MaxValue, 105, 180, byte.MaxValue);

	public static readonly Color IndianRed = FromRgba(205, 92, 92, byte.MaxValue);

	public static readonly Color Indigo = FromRgba(75, 0, 130, byte.MaxValue);

	public static readonly Color Ivory = FromRgba(byte.MaxValue, byte.MaxValue, 240, byte.MaxValue);

	public static readonly Color Khaki = FromRgba(240, 230, 140, byte.MaxValue);

	public static readonly Color Lavender = FromRgba(230, 230, 250, byte.MaxValue);

	public static readonly Color LavenderBlush = FromRgba(byte.MaxValue, 240, 245, byte.MaxValue);

	public static readonly Color LawnGreen = FromRgba(124, 252, 0, byte.MaxValue);

	public static readonly Color LemonChiffon = FromRgba(byte.MaxValue, 250, 205, byte.MaxValue);

	public static readonly Color LightBlue = FromRgba(173, 216, 230, byte.MaxValue);

	public static readonly Color LightCoral = FromRgba(240, 128, 128, byte.MaxValue);

	public static readonly Color LightCyan = FromRgba(224, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color LightGoldenrodYellow = FromRgba(250, 250, 210, byte.MaxValue);

	public static readonly Color LightGray = FromRgba(211, 211, 211, byte.MaxValue);

	public static readonly Color LightGreen = FromRgba(144, 238, 144, byte.MaxValue);

	public static readonly Color LightGrey = LightGray;

	public static readonly Color LightPink = FromRgba(byte.MaxValue, 182, 193, byte.MaxValue);

	public static readonly Color LightSalmon = FromRgba(byte.MaxValue, 160, 122, byte.MaxValue);

	public static readonly Color LightSeaGreen = FromRgba(32, 178, 170, byte.MaxValue);

	public static readonly Color LightSkyBlue = FromRgba(135, 206, 250, byte.MaxValue);

	public static readonly Color LightSlateGray = FromRgba(119, 136, 153, byte.MaxValue);

	public static readonly Color LightSlateGrey = LightSlateGray;

	public static readonly Color LightSteelBlue = FromRgba(176, 196, 222, byte.MaxValue);

	public static readonly Color LightYellow = FromRgba(byte.MaxValue, byte.MaxValue, 224, byte.MaxValue);

	public static readonly Color Lime = FromRgba(0, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color LimeGreen = FromRgba(50, 205, 50, byte.MaxValue);

	public static readonly Color Linen = FromRgba(250, 240, 230, byte.MaxValue);

	public static readonly Color Magenta = Fuchsia;

	public static readonly Color Maroon = FromRgba(128, 0, 0, byte.MaxValue);

	public static readonly Color MediumAquamarine = FromRgba(102, 205, 170, byte.MaxValue);

	public static readonly Color MediumBlue = FromRgba(0, 0, 205, byte.MaxValue);

	public static readonly Color MediumOrchid = FromRgba(186, 85, 211, byte.MaxValue);

	public static readonly Color MediumPurple = FromRgba(147, 112, 219, byte.MaxValue);

	public static readonly Color MediumSeaGreen = FromRgba(60, 179, 113, byte.MaxValue);

	public static readonly Color MediumSlateBlue = FromRgba(123, 104, 238, byte.MaxValue);

	public static readonly Color MediumSpringGreen = FromRgba(0, 250, 154, byte.MaxValue);

	public static readonly Color MediumTurquoise = FromRgba(72, 209, 204, byte.MaxValue);

	public static readonly Color MediumVioletRed = FromRgba(199, 21, 133, byte.MaxValue);

	public static readonly Color MidnightBlue = FromRgba(25, 25, 112, byte.MaxValue);

	public static readonly Color MintCream = FromRgba(245, byte.MaxValue, 250, byte.MaxValue);

	public static readonly Color MistyRose = FromRgba(byte.MaxValue, 228, 225, byte.MaxValue);

	public static readonly Color Moccasin = FromRgba(byte.MaxValue, 228, 181, byte.MaxValue);

	public static readonly Color NavajoWhite = FromRgba(byte.MaxValue, 222, 173, byte.MaxValue);

	public static readonly Color Navy = FromRgba(0, 0, 128, byte.MaxValue);

	public static readonly Color OldLace = FromRgba(253, 245, 230, byte.MaxValue);

	public static readonly Color Olive = FromRgba(128, 128, 0, byte.MaxValue);

	public static readonly Color OliveDrab = FromRgba(107, 142, 35, byte.MaxValue);

	public static readonly Color Orange = FromRgba(byte.MaxValue, 165, 0, byte.MaxValue);

	public static readonly Color OrangeRed = FromRgba(byte.MaxValue, 69, 0, byte.MaxValue);

	public static readonly Color Orchid = FromRgba(218, 112, 214, byte.MaxValue);

	public static readonly Color PaleGoldenrod = FromRgba(238, 232, 170, byte.MaxValue);

	public static readonly Color PaleGreen = FromRgba(152, 251, 152, byte.MaxValue);

	public static readonly Color PaleTurquoise = FromRgba(175, 238, 238, byte.MaxValue);

	public static readonly Color PaleVioletRed = FromRgba(219, 112, 147, byte.MaxValue);

	public static readonly Color PapayaWhip = FromRgba(byte.MaxValue, 239, 213, byte.MaxValue);

	public static readonly Color PeachPuff = FromRgba(byte.MaxValue, 218, 185, byte.MaxValue);

	public static readonly Color Peru = FromRgba(205, 133, 63, byte.MaxValue);

	public static readonly Color Pink = FromRgba(byte.MaxValue, 192, 203, byte.MaxValue);

	public static readonly Color Plum = FromRgba(221, 160, 221, byte.MaxValue);

	public static readonly Color PowderBlue = FromRgba(176, 224, 230, byte.MaxValue);

	public static readonly Color Purple = FromRgba(128, 0, 128, byte.MaxValue);

	public static readonly Color RebeccaPurple = FromRgba(102, 51, 153, byte.MaxValue);

	public static readonly Color Red = FromRgba(byte.MaxValue, 0, 0, byte.MaxValue);

	public static readonly Color RosyBrown = FromRgba(188, 143, 143, byte.MaxValue);

	public static readonly Color RoyalBlue = FromRgba(65, 105, 225, byte.MaxValue);

	public static readonly Color SaddleBrown = FromRgba(139, 69, 19, byte.MaxValue);

	public static readonly Color Salmon = FromRgba(250, 128, 114, byte.MaxValue);

	public static readonly Color SandyBrown = FromRgba(244, 164, 96, byte.MaxValue);

	public static readonly Color SeaGreen = FromRgba(46, 139, 87, byte.MaxValue);

	public static readonly Color SeaShell = FromRgba(byte.MaxValue, 245, 238, byte.MaxValue);

	public static readonly Color Sienna = FromRgba(160, 82, 45, byte.MaxValue);

	public static readonly Color Silver = FromRgba(192, 192, 192, byte.MaxValue);

	public static readonly Color SkyBlue = FromRgba(135, 206, 235, byte.MaxValue);

	public static readonly Color SlateBlue = FromRgba(106, 90, 205, byte.MaxValue);

	public static readonly Color SlateGray = FromRgba(112, 128, 144, byte.MaxValue);

	public static readonly Color SlateGrey = SlateGray;

	public static readonly Color Snow = FromRgba(byte.MaxValue, 250, 250, byte.MaxValue);

	public static readonly Color SpringGreen = FromRgba(0, byte.MaxValue, 127, byte.MaxValue);

	public static readonly Color SteelBlue = FromRgba(70, 130, 180, byte.MaxValue);

	public static readonly Color Tan = FromRgba(210, 180, 140, byte.MaxValue);

	public static readonly Color Teal = FromRgba(0, 128, 128, byte.MaxValue);

	public static readonly Color Thistle = FromRgba(216, 191, 216, byte.MaxValue);

	public static readonly Color Tomato = FromRgba(byte.MaxValue, 99, 71, byte.MaxValue);

	public static readonly Color Transparent = FromRgba(0, 0, 0, 0);

	public static readonly Color Turquoise = FromRgba(64, 224, 208, byte.MaxValue);

	public static readonly Color Violet = FromRgba(238, 130, 238, byte.MaxValue);

	public static readonly Color Wheat = FromRgba(245, 222, 179, byte.MaxValue);

	public static readonly Color White = FromRgba(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color WhiteSmoke = FromRgba(245, 245, 245, byte.MaxValue);

	public static readonly Color Yellow = FromRgba(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color YellowGreen = FromRgba(154, 205, 50, byte.MaxValue);

	private static readonly Lazy<Color[]> WebSafePaletteLazy = new Lazy<Color[]>(CreateWebSafePalette, isThreadSafe: true);

	private static readonly Lazy<Color[]> WernerPaletteLazy = new Lazy<Color[]>(CreateWernerPalette, isThreadSafe: true);

	public static ReadOnlyMemory<Color> WebSafePalette => WebSafePaletteLazy.Value;

	public static ReadOnlyMemory<Color> WernerPalette => WernerPaletteLazy.Value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Rgba64 pixel)
	{
		data = pixel;
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Rgb48 pixel)
	{
		data = new Rgba64(pixel.R, pixel.G, pixel.B, ushort.MaxValue);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(La32 pixel)
	{
		data = new Rgba64(pixel.L, pixel.L, pixel.L, pixel.A);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(L16 pixel)
	{
		data = new Rgba64(pixel.PackedValue, pixel.PackedValue, pixel.PackedValue, ushort.MaxValue);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Rgba32 pixel)
	{
		data = new Rgba64(pixel);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Argb32 pixel)
	{
		data = new Rgba64(pixel);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Bgra32 pixel)
	{
		data = new Rgba64(pixel);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Abgr32 pixel)
	{
		data = new Rgba64(pixel);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Rgb24 pixel)
	{
		data = new Rgba64(pixel);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Bgr24 pixel)
	{
		data = new Rgba64(pixel);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color(Vector4 vector)
	{
		vector = Numerics.Clamp(vector, Vector4.Zero, Vector4.One);
		boxedHighPrecisionPixel = new RgbaVector(vector.X, vector.Y, vector.Z, vector.W);
		data = default(Rgba64);
	}

	public static explicit operator Vector4(Color color)
	{
		return color.ToScaledVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Color(Vector4 source)
	{
		return new Color(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Rgba32 ToRgba32()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToRgba32();
		}
		Rgba32 dest = default(Rgba32);
		boxedHighPrecisionPixel.ToRgba32(ref dest);
		return dest;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Bgra32 ToBgra32()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToBgra32();
		}
		Bgra32 result = default(Bgra32);
		result.FromScaledVector4(boxedHighPrecisionPixel.ToScaledVector4());
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Argb32 ToArgb32()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToArgb32();
		}
		Argb32 result = default(Argb32);
		result.FromScaledVector4(boxedHighPrecisionPixel.ToScaledVector4());
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Abgr32 ToAbgr32()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToAbgr32();
		}
		Abgr32 result = default(Abgr32);
		result.FromScaledVector4(boxedHighPrecisionPixel.ToScaledVector4());
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Rgb24 ToRgb24()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToRgb24();
		}
		Rgb24 result = default(Rgb24);
		result.FromScaledVector4(boxedHighPrecisionPixel.ToScaledVector4());
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Bgr24 ToBgr24()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToBgr24();
		}
		Bgr24 result = default(Bgr24);
		result.FromScaledVector4(boxedHighPrecisionPixel.ToScaledVector4());
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Vector4 ToScaledVector4()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.ToScaledVector4();
		}
		return boxedHighPrecisionPixel.ToScaledVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color(byte r, byte g, byte b, byte a)
	{
		data = new Rgba64(ColorNumerics.UpscaleFrom8BitTo16Bit(r), ColorNumerics.UpscaleFrom8BitTo16Bit(g), ColorNumerics.UpscaleFrom8BitTo16Bit(b), ColorNumerics.UpscaleFrom8BitTo16Bit(a));
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color(byte r, byte g, byte b)
	{
		data = new Rgba64(ColorNumerics.UpscaleFrom8BitTo16Bit(r), ColorNumerics.UpscaleFrom8BitTo16Bit(g), ColorNumerics.UpscaleFrom8BitTo16Bit(b), ushort.MaxValue);
		boxedHighPrecisionPixel = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color(IPixel pixel)
	{
		boxedHighPrecisionPixel = pixel;
		data = default(Rgba64);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Color left, Color right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Color left, Color right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color FromRgba(byte r, byte g, byte b, byte a)
	{
		return new Color(r, g, b, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color FromRgb(byte r, byte g, byte b)
	{
		return new Color(r, g, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color FromPixel<TPixel>(TPixel pixel) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (typeof(TPixel) == typeof(Rgba64))
		{
			return new Color((Rgba64)(object)pixel);
		}
		if (typeof(TPixel) == typeof(Rgb48))
		{
			return new Color((Rgb48)(object)pixel);
		}
		if (typeof(TPixel) == typeof(La32))
		{
			return new Color((La32)(object)pixel);
		}
		if (typeof(TPixel) == typeof(L16))
		{
			return new Color((L16)(object)pixel);
		}
		if (Unsafe.SizeOf<TPixel>() <= Unsafe.SizeOf<Rgba32>())
		{
			Rgba32 dest = default(Rgba32);
			pixel.ToRgba32(ref dest);
			return new Color(dest);
		}
		return new Color(pixel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color ParseHex(string hex)
	{
		return new Color(Rgba32.ParseHex(hex));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseHex(string hex, out Color result)
	{
		result = default(Color);
		if (Rgba32.TryParseHex(hex, out var result2))
		{
			result = new Color(result2);
			return true;
		}
		return false;
	}

	public static Color Parse(string input)
	{
		Guard.NotNull(input, "input");
		if (!TryParse(input, out var result))
		{
			throw new ArgumentException("Input string is not in the correct format.", "input");
		}
		return result;
	}

	public static bool TryParse(string input, out Color result)
	{
		result = default(Color);
		if (string.IsNullOrWhiteSpace(input))
		{
			return false;
		}
		if (NamedColorsLookupLazy.Value.TryGetValue(input, out result))
		{
			return true;
		}
		return TryParseHex(input, out result);
	}

	public Color WithAlpha(float alpha)
	{
		Vector4 vector = (Vector4)this;
		vector.W = alpha;
		return new Color(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToHex()
	{
		if (boxedHighPrecisionPixel != null)
		{
			Rgba32 dest = default(Rgba32);
			boxedHighPrecisionPixel.ToRgba32(ref dest);
			return dest.ToHex();
		}
		return data.ToRgba32().ToHex();
	}

	public override string ToString()
	{
		return ToHex();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TPixel ToPixel<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
	{
		IPixel pixel = boxedHighPrecisionPixel;
		if (pixel is TPixel)
		{
			return (TPixel)pixel;
		}
		TPixel result;
		if (boxedHighPrecisionPixel == null)
		{
			result = default(TPixel);
			result.FromRgba64(data);
			return result;
		}
		result = default(TPixel);
		result.FromScaledVector4(boxedHighPrecisionPixel.ToScaledVector4());
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ToPixel<TPixel>(ReadOnlySpan<Color> source, Span<TPixel> destination) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.DestinationShouldNotBeTooShort(source, destination, "destination");
		for (int i = 0; i < source.Length; i++)
		{
			destination[i] = source[i].ToPixel<TPixel>();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Color other)
	{
		if (boxedHighPrecisionPixel == null && other.boxedHighPrecisionPixel == null)
		{
			return data.PackedValue == other.data.PackedValue;
		}
		return boxedHighPrecisionPixel?.Equals(other.boxedHighPrecisionPixel) ?? false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Color other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		if (boxedHighPrecisionPixel == null)
		{
			return data.PackedValue.GetHashCode();
		}
		return boxedHighPrecisionPixel.GetHashCode();
	}

	private static Dictionary<string, Color> CreateNamedColorsLookup()
	{
		return new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
		{
			{ "AliceBlue", AliceBlue },
			{ "AntiqueWhite", AntiqueWhite },
			{ "Aqua", Aqua },
			{ "Aquamarine", Aquamarine },
			{ "Azure", Azure },
			{ "Beige", Beige },
			{ "Bisque", Bisque },
			{ "Black", Black },
			{ "BlanchedAlmond", BlanchedAlmond },
			{ "Blue", Blue },
			{ "BlueViolet", BlueViolet },
			{ "Brown", Brown },
			{ "BurlyWood", BurlyWood },
			{ "CadetBlue", CadetBlue },
			{ "Chartreuse", Chartreuse },
			{ "Chocolate", Chocolate },
			{ "Coral", Coral },
			{ "CornflowerBlue", CornflowerBlue },
			{ "Cornsilk", Cornsilk },
			{ "Crimson", Crimson },
			{ "Cyan", Cyan },
			{ "DarkBlue", DarkBlue },
			{ "DarkCyan", DarkCyan },
			{ "DarkGoldenrod", DarkGoldenrod },
			{ "DarkGray", DarkGray },
			{ "DarkGreen", DarkGreen },
			{ "DarkGrey", DarkGrey },
			{ "DarkKhaki", DarkKhaki },
			{ "DarkMagenta", DarkMagenta },
			{ "DarkOliveGreen", DarkOliveGreen },
			{ "DarkOrange", DarkOrange },
			{ "DarkOrchid", DarkOrchid },
			{ "DarkRed", DarkRed },
			{ "DarkSalmon", DarkSalmon },
			{ "DarkSeaGreen", DarkSeaGreen },
			{ "DarkSlateBlue", DarkSlateBlue },
			{ "DarkSlateGray", DarkSlateGray },
			{ "DarkSlateGrey", DarkSlateGrey },
			{ "DarkTurquoise", DarkTurquoise },
			{ "DarkViolet", DarkViolet },
			{ "DeepPink", DeepPink },
			{ "DeepSkyBlue", DeepSkyBlue },
			{ "DimGray", DimGray },
			{ "DimGrey", DimGrey },
			{ "DodgerBlue", DodgerBlue },
			{ "Firebrick", Firebrick },
			{ "FloralWhite", FloralWhite },
			{ "ForestGreen", ForestGreen },
			{ "Fuchsia", Fuchsia },
			{ "Gainsboro", Gainsboro },
			{ "GhostWhite", GhostWhite },
			{ "Gold", Gold },
			{ "Goldenrod", Goldenrod },
			{ "Gray", Gray },
			{ "Green", Green },
			{ "GreenYellow", GreenYellow },
			{ "Grey", Grey },
			{ "Honeydew", Honeydew },
			{ "HotPink", HotPink },
			{ "IndianRed", IndianRed },
			{ "Indigo", Indigo },
			{ "Ivory", Ivory },
			{ "Khaki", Khaki },
			{ "Lavender", Lavender },
			{ "LavenderBlush", LavenderBlush },
			{ "LawnGreen", LawnGreen },
			{ "LemonChiffon", LemonChiffon },
			{ "LightBlue", LightBlue },
			{ "LightCoral", LightCoral },
			{ "LightCyan", LightCyan },
			{ "LightGoldenrodYellow", LightGoldenrodYellow },
			{ "LightGray", LightGray },
			{ "LightGreen", LightGreen },
			{ "LightGrey", LightGrey },
			{ "LightPink", LightPink },
			{ "LightSalmon", LightSalmon },
			{ "LightSeaGreen", LightSeaGreen },
			{ "LightSkyBlue", LightSkyBlue },
			{ "LightSlateGray", LightSlateGray },
			{ "LightSlateGrey", LightSlateGrey },
			{ "LightSteelBlue", LightSteelBlue },
			{ "LightYellow", LightYellow },
			{ "Lime", Lime },
			{ "LimeGreen", LimeGreen },
			{ "Linen", Linen },
			{ "Magenta", Magenta },
			{ "Maroon", Maroon },
			{ "MediumAquamarine", MediumAquamarine },
			{ "MediumBlue", MediumBlue },
			{ "MediumOrchid", MediumOrchid },
			{ "MediumPurple", MediumPurple },
			{ "MediumSeaGreen", MediumSeaGreen },
			{ "MediumSlateBlue", MediumSlateBlue },
			{ "MediumSpringGreen", MediumSpringGreen },
			{ "MediumTurquoise", MediumTurquoise },
			{ "MediumVioletRed", MediumVioletRed },
			{ "MidnightBlue", MidnightBlue },
			{ "MintCream", MintCream },
			{ "MistyRose", MistyRose },
			{ "Moccasin", Moccasin },
			{ "NavajoWhite", NavajoWhite },
			{ "Navy", Navy },
			{ "OldLace", OldLace },
			{ "Olive", Olive },
			{ "OliveDrab", OliveDrab },
			{ "Orange", Orange },
			{ "OrangeRed", OrangeRed },
			{ "Orchid", Orchid },
			{ "PaleGoldenrod", PaleGoldenrod },
			{ "PaleGreen", PaleGreen },
			{ "PaleTurquoise", PaleTurquoise },
			{ "PaleVioletRed", PaleVioletRed },
			{ "PapayaWhip", PapayaWhip },
			{ "PeachPuff", PeachPuff },
			{ "Peru", Peru },
			{ "Pink", Pink },
			{ "Plum", Plum },
			{ "PowderBlue", PowderBlue },
			{ "Purple", Purple },
			{ "RebeccaPurple", RebeccaPurple },
			{ "Red", Red },
			{ "RosyBrown", RosyBrown },
			{ "RoyalBlue", RoyalBlue },
			{ "SaddleBrown", SaddleBrown },
			{ "Salmon", Salmon },
			{ "SandyBrown", SandyBrown },
			{ "SeaGreen", SeaGreen },
			{ "SeaShell", SeaShell },
			{ "Sienna", Sienna },
			{ "Silver", Silver },
			{ "SkyBlue", SkyBlue },
			{ "SlateBlue", SlateBlue },
			{ "SlateGray", SlateGray },
			{ "SlateGrey", SlateGrey },
			{ "Snow", Snow },
			{ "SpringGreen", SpringGreen },
			{ "SteelBlue", SteelBlue },
			{ "Tan", Tan },
			{ "Teal", Teal },
			{ "Thistle", Thistle },
			{ "Tomato", Tomato },
			{ "Transparent", Transparent },
			{ "Turquoise", Turquoise },
			{ "Violet", Violet },
			{ "Wheat", Wheat },
			{ "White", White },
			{ "WhiteSmoke", WhiteSmoke },
			{ "Yellow", Yellow },
			{ "YellowGreen", YellowGreen }
		};
	}

	private static Color[] CreateWebSafePalette()
	{
		return new Color[142]
		{
			AliceBlue, AntiqueWhite, Aqua, Aquamarine, Azure, Beige, Bisque, Black, BlanchedAlmond, Blue,
			BlueViolet, Brown, BurlyWood, CadetBlue, Chartreuse, Chocolate, Coral, CornflowerBlue, Cornsilk, Crimson,
			Cyan, DarkBlue, DarkCyan, DarkGoldenrod, DarkGray, DarkGreen, DarkKhaki, DarkMagenta, DarkOliveGreen, DarkOrange,
			DarkOrchid, DarkRed, DarkSalmon, DarkSeaGreen, DarkSlateBlue, DarkSlateGray, DarkTurquoise, DarkViolet, DeepPink, DeepSkyBlue,
			DimGray, DodgerBlue, Firebrick, FloralWhite, ForestGreen, Fuchsia, Gainsboro, GhostWhite, Gold, Goldenrod,
			Gray, Green, GreenYellow, Honeydew, HotPink, IndianRed, Indigo, Ivory, Khaki, Lavender,
			LavenderBlush, LawnGreen, LemonChiffon, LightBlue, LightCoral, LightCyan, LightGoldenrodYellow, LightGray, LightGreen, LightPink,
			LightSalmon, LightSeaGreen, LightSkyBlue, LightSlateGray, LightSteelBlue, LightYellow, Lime, LimeGreen, Linen, Magenta,
			Maroon, MediumAquamarine, MediumBlue, MediumOrchid, MediumPurple, MediumSeaGreen, MediumSlateBlue, MediumSpringGreen, MediumTurquoise, MediumVioletRed,
			MidnightBlue, MintCream, MistyRose, Moccasin, NavajoWhite, Navy, OldLace, Olive, OliveDrab, Orange,
			OrangeRed, Orchid, PaleGoldenrod, PaleGreen, PaleTurquoise, PaleVioletRed, PapayaWhip, PeachPuff, Peru, Pink,
			Plum, PowderBlue, Purple, RebeccaPurple, Red, RosyBrown, RoyalBlue, SaddleBrown, Salmon, SandyBrown,
			SeaGreen, SeaShell, Sienna, Silver, SkyBlue, SlateBlue, SlateGray, Snow, SpringGreen, SteelBlue,
			Tan, Teal, Thistle, Tomato, Transparent, Turquoise, Violet, Wheat, White, WhiteSmoke,
			Yellow, YellowGreen
		};
	}

	private static Color[] CreateWernerPalette()
	{
		return new Color[110]
		{
			ParseHex("#f1e9cd"),
			ParseHex("#f2e7cf"),
			ParseHex("#ece6d0"),
			ParseHex("#f2eacc"),
			ParseHex("#f3e9ca"),
			ParseHex("#f2ebcd"),
			ParseHex("#e6e1c9"),
			ParseHex("#e2ddc6"),
			ParseHex("#cbc8b7"),
			ParseHex("#bfbbb0"),
			ParseHex("#bebeb3"),
			ParseHex("#b7b5ac"),
			ParseHex("#bab191"),
			ParseHex("#9c9d9a"),
			ParseHex("#8a8d84"),
			ParseHex("#5b5c61"),
			ParseHex("#555152"),
			ParseHex("#413f44"),
			ParseHex("#454445"),
			ParseHex("#423937"),
			ParseHex("#433635"),
			ParseHex("#252024"),
			ParseHex("#241f20"),
			ParseHex("#281f3f"),
			ParseHex("#1c1949"),
			ParseHex("#4f638d"),
			ParseHex("#383867"),
			ParseHex("#5c6b8f"),
			ParseHex("#657abb"),
			ParseHex("#6f88af"),
			ParseHex("#7994b5"),
			ParseHex("#6fb5a8"),
			ParseHex("#719ba2"),
			ParseHex("#8aa1a6"),
			ParseHex("#d0d5d3"),
			ParseHex("#8590ae"),
			ParseHex("#3a2f52"),
			ParseHex("#39334a"),
			ParseHex("#6c6d94"),
			ParseHex("#584c77"),
			ParseHex("#533552"),
			ParseHex("#463759"),
			ParseHex("#bfbac0"),
			ParseHex("#77747f"),
			ParseHex("#4a475c"),
			ParseHex("#b8bfaf"),
			ParseHex("#b2b599"),
			ParseHex("#979c84"),
			ParseHex("#5d6161"),
			ParseHex("#61ac86"),
			ParseHex("#a4b6a7"),
			ParseHex("#adba98"),
			ParseHex("#93b778"),
			ParseHex("#7d8c55"),
			ParseHex("#33431e"),
			ParseHex("#7c8635"),
			ParseHex("#8e9849"),
			ParseHex("#c2c190"),
			ParseHex("#67765b"),
			ParseHex("#ab924b"),
			ParseHex("#c8c76f"),
			ParseHex("#ccc050"),
			ParseHex("#ebdd99"),
			ParseHex("#ab9649"),
			ParseHex("#dbc364"),
			ParseHex("#e6d058"),
			ParseHex("#ead665"),
			ParseHex("#d09b2c"),
			ParseHex("#a36629"),
			ParseHex("#a77d35"),
			ParseHex("#f0d696"),
			ParseHex("#d7c485"),
			ParseHex("#f1d28c"),
			ParseHex("#efcc83"),
			ParseHex("#f3daa7"),
			ParseHex("#dfa837"),
			ParseHex("#ebbc71"),
			ParseHex("#d17c3f"),
			ParseHex("#92462f"),
			ParseHex("#be7249"),
			ParseHex("#bb603c"),
			ParseHex("#c76b4a"),
			ParseHex("#a75536"),
			ParseHex("#b63e36"),
			ParseHex("#b5493a"),
			ParseHex("#cd6d57"),
			ParseHex("#711518"),
			ParseHex("#e9c49d"),
			ParseHex("#eedac3"),
			ParseHex("#eecfbf"),
			ParseHex("#ce536b"),
			ParseHex("#b74a70"),
			ParseHex("#b7757c"),
			ParseHex("#612741"),
			ParseHex("#7a4848"),
			ParseHex("#3f3033"),
			ParseHex("#8d746f"),
			ParseHex("#4d3635"),
			ParseHex("#6e3b31"),
			ParseHex("#864735"),
			ParseHex("#553d3a"),
			ParseHex("#613936"),
			ParseHex("#7a4b3a"),
			ParseHex("#946943"),
			ParseHex("#c39e6d"),
			ParseHex("#513e32"),
			ParseHex("#8b7859"),
			ParseHex("#9b856b"),
			ParseHex("#766051"),
			ParseHex("#453b32")
		};
	}
}
