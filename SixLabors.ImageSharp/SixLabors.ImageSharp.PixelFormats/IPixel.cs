using System;
using System.Numerics;

namespace SixLabors.ImageSharp.PixelFormats;

public interface IPixel<TSelf> : IPixel, IEquatable<TSelf> where TSelf : unmanaged, IPixel<TSelf>
{
	PixelOperations<TSelf> CreatePixelOperations();
}
public interface IPixel
{
	void FromScaledVector4(Vector4 vector);

	Vector4 ToScaledVector4();

	void FromVector4(Vector4 vector);

	Vector4 ToVector4();

	void FromArgb32(Argb32 source);

	void FromBgra5551(Bgra5551 source);

	void FromBgr24(Bgr24 source);

	void FromBgra32(Bgra32 source);

	void FromAbgr32(Abgr32 source);

	void FromL8(L8 source);

	void FromL16(L16 source);

	void FromLa16(La16 source);

	void FromLa32(La32 source);

	void FromRgb24(Rgb24 source);

	void FromRgba32(Rgba32 source);

	void ToRgba32(ref Rgba32 dest);

	void FromRgb48(Rgb48 source);

	void FromRgba64(Rgba64 source);
}
