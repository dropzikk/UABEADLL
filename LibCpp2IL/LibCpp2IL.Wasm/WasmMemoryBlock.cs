using System.IO;
using System.Linq;

namespace LibCpp2IL.Wasm;

public class WasmMemoryBlock : ClassReadingBinaryReader
{
	internal byte[] Bytes;

	private static MemoryStream BuildStream(WasmFile file)
	{
		ulong num = ((from s in file.DataSection.DataEntries
			where s.VirtualOffset != ulong.MaxValue
			select s.VirtualOffset + s.Size).Max() + 4096) * 2;
		MemoryStream memoryStream = new MemoryStream(new byte[num], 0, (int)num, writable: true, publiclyVisible: true);
		foreach (WasmDataSegment dataEntry in file.DataSection.DataEntries)
		{
			memoryStream.Seek((long)dataEntry.VirtualOffset, SeekOrigin.Begin);
			memoryStream.Write(dataEntry.Data, 0, (int)dataEntry.Size);
		}
		return memoryStream;
	}

	public WasmMemoryBlock(WasmFile file)
		: base(BuildStream(file))
	{
		is32Bit = true;
		Bytes = ((MemoryStream)BaseStream).ToArray();
	}
}
