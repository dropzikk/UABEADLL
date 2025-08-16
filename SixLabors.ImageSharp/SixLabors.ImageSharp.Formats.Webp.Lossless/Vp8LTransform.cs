using System.Buffers;
using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("Transformtype: {TransformType}")]
internal class Vp8LTransform
{
	public Vp8LTransformType TransformType { get; }

	public int Bits { get; set; }

	public int XSize { get; set; }

	public int YSize { get; }

	public IMemoryOwner<uint> Data { get; set; }

	public Vp8LTransform(Vp8LTransformType transformType, int xSize, int ySize)
	{
		TransformType = transformType;
		XSize = xSize;
		YSize = ySize;
	}
}
