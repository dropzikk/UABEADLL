using System;

namespace SixLabors.ImageSharp.PixelFormats;

public interface IPackedVector<TPacked> : IPixel where TPacked : struct, IEquatable<TPacked>
{
	TPacked PackedValue { get; set; }
}
