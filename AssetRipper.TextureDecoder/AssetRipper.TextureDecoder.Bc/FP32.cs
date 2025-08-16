using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Bc;

[StructLayout(LayoutKind.Explicit)]
internal struct FP32
{
	[FieldOffset(0)]
	public uint u;

	[FieldOffset(0)]
	public float f;
}
