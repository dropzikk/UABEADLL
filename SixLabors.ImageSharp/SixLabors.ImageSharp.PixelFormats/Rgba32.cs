using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats.Utils;

namespace SixLabors.ImageSharp.PixelFormats;

public struct Rgba32 : IPixel<Rgba32>, IPixel, IEquatable<Rgba32>, IPackedVector<uint>
{
	internal class PixelOperations : PixelOperations<Rgba32>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<Rgba32>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override void FromRgba32(Configuration configuration, ReadOnlySpan<Rgba32> source, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(source, destinationPixels, "destinationPixels");
			source.CopyTo(destinationPixels.Slice(0, source.Length));
		}

		public override void ToRgba32(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			sourcePixels.CopyTo(destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override void ToArgb32(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Argb32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgba32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Argb32, byte>(destinationPixels);
			PixelConverter.FromRgba32.ToArgb32(source, dest);
		}

		public override void FromArgb32(Configuration configuration, ReadOnlySpan<Argb32> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Argb32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgba32, byte>(destinationPixels);
			PixelConverter.FromArgb32.ToRgba32(source, dest);
		}

		public override void ToAbgr32(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Abgr32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgba32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Abgr32, byte>(destinationPixels);
			PixelConverter.FromRgba32.ToAbgr32(source, dest);
		}

		public override void FromAbgr32(Configuration configuration, ReadOnlySpan<Abgr32> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Abgr32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgba32, byte>(destinationPixels);
			PixelConverter.FromAbgr32.ToRgba32(source, dest);
		}

		public override void ToBgra32(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Bgra32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgba32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgra32, byte>(destinationPixels);
			PixelConverter.FromRgba32.ToBgra32(source, dest);
		}

		public override void FromBgra32(Configuration configuration, ReadOnlySpan<Bgra32> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgra32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgba32, byte>(destinationPixels);
			PixelConverter.FromBgra32.ToRgba32(source, dest);
		}

		public override void ToRgb24(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Rgb24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgba32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgb24, byte>(destinationPixels);
			PixelConverter.FromRgba32.ToRgb24(source, dest);
		}

		public override void FromRgb24(Configuration configuration, ReadOnlySpan<Rgb24> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgb24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgba32, byte>(destinationPixels);
			PixelConverter.FromRgb24.ToRgba32(source, dest);
		}

		public override void ToBgr24(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Bgr24> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Rgba32, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Bgr24, byte>(destinationPixels);
			PixelConverter.FromRgba32.ToBgr24(source, dest);
		}

		public override void FromBgr24(Configuration configuration, ReadOnlySpan<Bgr24> sourcePixels, Span<Rgba32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ReadOnlySpan<byte> source = MemoryMarshal.Cast<Bgr24, byte>(sourcePixels);
			Span<byte> dest = MemoryMarshal.Cast<Rgba32, byte>(destinationPixels);
			PixelConverter.FromBgr24.ToRgba32(source, dest);
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L8 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref L16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void ToLa16(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<La16> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La16 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void ToLa32(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<La32> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref La32 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void ToRgb48(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Rgb48> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgb48 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void ToRgba64(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Rgba64> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Rgba64 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void ToBgra5551(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Bgra5551> destinationPixels)
		{
			Guard.NotNull(configuration, "configuration");
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Rgba32 reference = ref MemoryMarshal.GetReference(sourcePixels);
			ref Bgra5551 reference2 = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Rgba32 reference3 = ref Unsafe.Add(ref reference, num);
				Unsafe.Add(ref reference2, num).FromRgba32(reference3);
			}
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<Rgba32> destinationPixels)
		{
			PixelOperations<TSourcePixel>.Instance.ToRgba32(configuration, sourcePixels, destinationPixels.Slice(0, sourcePixels.Length));
		}

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}

		public override void ToVector4(Configuration configuration, ReadOnlySpan<Rgba32> sourcePixels, Span<Vector4> destinationVectors, PixelConversionModifiers modifiers)
		{
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationVectors, "destinationVectors");
			destinationVectors = destinationVectors.Slice(0, sourcePixels.Length);
			SimdUtils.ByteToNormalizedFloat(MemoryMarshal.Cast<Rgba32, byte>(sourcePixels), MemoryMarshal.Cast<Vector4, float>(destinationVectors));
			Vector4Converters.ApplyForwardConversionModifiers(destinationVectors, modifiers);
		}

		public override void FromVector4Destructive(Configuration configuration, Span<Vector4> sourceVectors, Span<Rgba32> destinationPixels, PixelConversionModifiers modifiers)
		{
			Guard.DestinationShouldNotBeTooShort(sourceVectors, destinationPixels, "destinationPixels");
			destinationPixels = destinationPixels.Slice(0, sourceVectors.Length);
			Vector4Converters.ApplyBackwardConversionModifiers(sourceVectors, modifiers);
			SimdUtils.NormalizedFloatToByteSaturate(MemoryMarshal.Cast<Vector4, float>(sourceVectors), MemoryMarshal.Cast<Rgba32, byte>(destinationPixels));
		}

		internal override void PackFromRgbPlanes(ReadOnlySpan<byte> redChannel, ReadOnlySpan<byte> greenChannel, ReadOnlySpan<byte> blueChannel, Span<Rgba32> destination)
		{
			int length = redChannel.Length;
			PixelOperations<Rgba32>.GuardPackFromRgbPlanes(greenChannel, blueChannel, destination, length);
			SimdUtils.PackFromRgbPlanes(redChannel, greenChannel, blueChannel, destination);
		}
	}

	public byte R;

	public byte G;

	public byte B;

	public byte A;

	private static readonly Vector4 MaxBytes = new Vector4(255f);

	private static readonly Vector4 Half = new Vector4(0.5f);

	public uint Rgba
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Unsafe.As<Rgba32, uint>(ref Unsafe.AsRef(in this));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Unsafe.As<Rgba32, uint>(ref this) = value;
		}
	}

	public Rgb24 Rgb
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Rgb24(R, G, B);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			R = value.R;
			G = value.G;
			B = value.B;
		}
	}

	public Bgr24 Bgr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Bgr24(R, G, B);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			R = value.R;
			G = value.G;
			B = value.B;
		}
	}

	public uint PackedValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Rgba;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Rgba = value;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32(byte r, byte g, byte b)
	{
		R = r;
		G = g;
		B = b;
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32(byte r, byte g, byte b, byte a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32(float r, float g, float b, float a = 1f)
	{
		this = default(Rgba32);
		Pack(r, g, b, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32(Vector3 vector)
	{
		this = default(Rgba32);
		Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32(Vector4 vector)
	{
		this = default(Rgba32);
		this = PackNew(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32(uint packed)
	{
		this = default(Rgba32);
		Rgba = packed;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Color(Rgba32 source)
	{
		return new Color(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Rgba32(Color color)
	{
		return color.ToRgba32();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Rgba32(Rgb color)
	{
		Vector4 vector = new Vector4(color.ToVector3(), 1f);
		Rgba32 result = default(Rgba32);
		result.FromScaledVector4(vector);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Rgba32 left, Rgba32 right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Rgba32 left, Rgba32 right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rgba32 ParseHex(string hex)
	{
		Guard.NotNull(hex, "hex");
		if (!TryParseHex(hex, out var result))
		{
			throw new ArgumentException("Hexadecimal string is not in the correct format.", "hex");
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseHex(string? hex, out Rgba32 result)
	{
		result = default(Rgba32);
		if (string.IsNullOrWhiteSpace(hex))
		{
			return false;
		}
		hex = ToRgbaHex(hex);
		if (hex == null || !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result2))
		{
			return false;
		}
		result2 = BinaryPrimitives.ReverseEndianness(result2);
		result = Unsafe.As<uint, Rgba32>(ref result2);
		return true;
	}

	public readonly PixelOperations<Rgba32> CreatePixelOperations()
	{
		return new PixelOperations();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromScaledVector4(Vector4 vector)
	{
		FromVector4(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToScaledVector4()
	{
		return ToVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromVector4(Vector4 vector)
	{
		Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4((int)R, (int)G, (int)B, (int)A) / MaxBytes;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		R = source.R;
		G = source.G;
		B = source.B;
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		Bgr = source;
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		R = source.R;
		G = source.G;
		B = source.B;
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		R = source.R;
		G = source.G;
		B = source.B;
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		R = source.PackedValue;
		G = source.PackedValue;
		B = source.PackedValue;
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		B = (G = (R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.PackedValue)));
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		R = source.L;
		G = source.L;
		B = source.L;
		A = source.A;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		B = (G = (R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.L)));
		A = ColorNumerics.DownScaleFrom16BitTo8Bit(source.A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		Rgb = source;
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		this = source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest = this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.R);
		G = ColorNumerics.DownScaleFrom16BitTo8Bit(source.G);
		B = ColorNumerics.DownScaleFrom16BitTo8Bit(source.B);
		A = byte.MaxValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		R = ColorNumerics.DownScaleFrom16BitTo8Bit(source.R);
		G = ColorNumerics.DownScaleFrom16BitTo8Bit(source.G);
		B = ColorNumerics.DownScaleFrom16BitTo8Bit(source.B);
		A = ColorNumerics.DownScaleFrom16BitTo8Bit(source.A);
	}

	public readonly string ToHex()
	{
		return ((uint)(A | (B << 8) | (G << 16) | (R << 24))).ToString("X8", CultureInfo.InvariantCulture);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is Rgba32 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Rgba32 other)
	{
		return Rgba.Equals(other.Rgba);
	}

	public override readonly string ToString()
	{
		return $"Rgba32({R}, {G}, {B}, {A})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return Rgba.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Rgba32 PackNew(ref Vector4 vector)
	{
		vector *= MaxBytes;
		vector += Half;
		vector = Numerics.Clamp(vector, Vector4.Zero, MaxBytes);
		return new Rgba32((byte)vector.X, (byte)vector.Y, (byte)vector.Z, (byte)vector.W);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Pack(float x, float y, float z, float w)
	{
		Vector4 vector = new Vector4(x, y, z, w);
		Pack(ref vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Pack(ref Vector3 vector)
	{
		Vector4 vector2 = new Vector4(vector, 1f);
		Pack(ref vector2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Pack(ref Vector4 vector)
	{
		vector *= MaxBytes;
		vector += Half;
		vector = Numerics.Clamp(vector, Vector4.Zero, MaxBytes);
		R = (byte)vector.X;
		G = (byte)vector.Y;
		B = (byte)vector.Z;
		A = (byte)vector.W;
	}

	private static string? ToRgbaHex(string hex)
	{
		if (hex[0] == '#')
		{
			string text = hex;
			hex = text.Substring(1, text.Length - 1);
		}
		if (hex.Length == 8)
		{
			return hex;
		}
		if (hex.Length == 6)
		{
			return hex + "FF";
		}
		int length = hex.Length;
		if ((length < 3 || length > 4) ? true : false)
		{
			return null;
		}
		char c = hex[0];
		char c2 = hex[1];
		char c3 = hex[2];
		char c4 = ((hex.Length == 3) ? 'F' : hex[3]);
		return new string(new char[8] { c, c, c2, c2, c3, c3, c4, c4 });
	}
}
