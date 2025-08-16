using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats.Utils;

namespace SixLabors.ImageSharp.PixelFormats;

public struct RgbaVector : IPixel<RgbaVector>, IPixel, IEquatable<RgbaVector>
{
	internal class PixelOperations : PixelOperations<RgbaVector>
	{
		private static readonly Lazy<PixelTypeInfo> LazyInfo = new Lazy<PixelTypeInfo>(() => PixelTypeInfo.Create<RgbaVector>(PixelAlphaRepresentation.Unassociated), isThreadSafe: true);

		public override PixelTypeInfo GetPixelTypeInfo()
		{
			return LazyInfo.Value;
		}

		public override void From<TSourcePixel>(Configuration configuration, ReadOnlySpan<TSourcePixel> sourcePixels, Span<RgbaVector> destinationPixels)
		{
			Span<Vector4> destinationVectors = MemoryMarshal.Cast<RgbaVector, Vector4>(destinationPixels);
			PixelOperations<TSourcePixel>.Instance.ToVector4(configuration, sourcePixels, destinationVectors, PixelConversionModifiers.Scale);
		}

		public override void FromVector4Destructive(Configuration configuration, Span<Vector4> sourceVectors, Span<RgbaVector> destinationPixels, PixelConversionModifiers modifiers)
		{
			Guard.DestinationShouldNotBeTooShort(sourceVectors, destinationPixels, "destinationPixels");
			Vector4Converters.ApplyBackwardConversionModifiers(sourceVectors, modifiers);
			MemoryMarshal.Cast<Vector4, RgbaVector>(sourceVectors).CopyTo(destinationPixels);
		}

		public override void ToVector4(Configuration configuration, ReadOnlySpan<RgbaVector> sourcePixels, Span<Vector4> destinationVectors, PixelConversionModifiers modifiers)
		{
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationVectors, "destinationVectors");
			MemoryMarshal.Cast<RgbaVector, Vector4>(sourcePixels).CopyTo(destinationVectors);
			Vector4Converters.ApplyForwardConversionModifiers(destinationVectors, modifiers);
		}

		public override void ToL8(Configuration configuration, ReadOnlySpan<RgbaVector> sourcePixels, Span<L8> destinationPixels)
		{
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Vector4 source = ref Unsafe.As<RgbaVector, Vector4>(ref MemoryMarshal.GetReference(sourcePixels));
			ref L8 reference = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Vector4 reference2 = ref Unsafe.Add(ref source, num);
				Unsafe.Add(ref reference, num).ConvertFromRgbaScaledVector4(reference2);
			}
		}

		public override void ToL16(Configuration configuration, ReadOnlySpan<RgbaVector> sourcePixels, Span<L16> destinationPixels)
		{
			Guard.DestinationShouldNotBeTooShort(sourcePixels, destinationPixels, "destinationPixels");
			ref Vector4 source = ref Unsafe.As<RgbaVector, Vector4>(ref MemoryMarshal.GetReference(sourcePixels));
			ref L16 reference = ref MemoryMarshal.GetReference(destinationPixels);
			for (nuint num = 0u; num < (uint)sourcePixels.Length; num++)
			{
				ref Vector4 reference2 = ref Unsafe.Add(ref source, num);
				Unsafe.Add(ref reference, num).ConvertFromRgbaScaledVector4(reference2);
			}
		}
	}

	public float R;

	public float G;

	public float B;

	public float A;

	private const float MaxBytes = 255f;

	private static readonly Vector4 Max = new Vector4(255f);

	private static readonly Vector4 Half = new Vector4(0.5f);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RgbaVector(float r, float g, float b, float a = 1f)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(RgbaVector left, RgbaVector right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(RgbaVector left, RgbaVector right)
	{
		return !left.Equals(right);
	}

	public static RgbaVector FromHex(string hex)
	{
		return Color.ParseHex(hex).ToPixel<RgbaVector>();
	}

	public readonly PixelOperations<RgbaVector> CreatePixelOperations()
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
		vector = Numerics.Clamp(vector, Vector4.Zero, Vector4.One);
		R = vector.X;
		G = vector.Y;
		B = vector.Z;
		A = vector.W;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 ToVector4()
	{
		return new Vector4(R, G, B, A);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromArgb32(Argb32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgr24(Bgr24 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra32(Bgra32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromAbgr32(Abgr32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromBgra5551(Bgra5551 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL8(L8 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromL16(L16 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa16(La16 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromLa32(La32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb24(Rgb24 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba32(Rgba32 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToRgba32(ref Rgba32 dest)
	{
		dest.FromScaledVector4(ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgb48(Rgb48 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FromRgba64(Rgba64 source)
	{
		FromScaledVector4(source.ToScaledVector4());
	}

	public readonly string ToHex()
	{
		Vector4 vector = ToVector4() * Max;
		vector += Half;
		return ((uint)((byte)vector.W | ((byte)vector.Z << 8) | ((byte)vector.Y << 16) | ((byte)vector.X << 24))).ToString("X8", CultureInfo.InvariantCulture);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is RgbaVector other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(RgbaVector other)
	{
		if (R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B))
		{
			return A.Equals(other.A);
		}
		return false;
	}

	public override readonly string ToString()
	{
		return FormattableString.Invariant($"RgbaVector({R:#0.##}, {G:#0.##}, {B:#0.##}, {A:#0.##})");
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(R, G, B, A);
	}
}
