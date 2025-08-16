using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

internal class ArithmeticDecodingComponent : JpegComponent
{
	public int DcContext { get; set; }

	public ArithmeticStatistics DcStatistics { get; set; }

	public ArithmeticStatistics AcStatistics { get; set; }

	public ArithmeticDecodingComponent(MemoryAllocator memoryAllocator, JpegFrame frame, byte id, int horizontalFactor, int verticalFactor, byte quantizationTableIndex, int index)
		: base(memoryAllocator, frame, id, horizontalFactor, verticalFactor, quantizationTableIndex, index)
	{
	}
}
