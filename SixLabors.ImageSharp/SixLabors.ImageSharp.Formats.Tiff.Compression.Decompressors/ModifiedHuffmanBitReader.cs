using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class ModifiedHuffmanBitReader : T4BitReader
{
	public override bool HasMoreData
	{
		get
		{
			if (base.Position >= (ulong)((long)base.DataLength - 1L))
			{
				return (uint)(base.BitsRead - 1) < 6u;
			}
			return true;
		}
	}

	public override bool IsEndOfScanLine
	{
		get
		{
			if (base.IsWhiteRun && base.CurValueBitsRead == 12 && base.Value == 1)
			{
				return true;
			}
			if (base.CurValueBitsRead == 11 && base.Value == 0)
			{
				return true;
			}
			return false;
		}
	}

	public ModifiedHuffmanBitReader(BufferedReadStream input, TiffFillOrder fillOrder, int bytesToRead)
		: base(input, fillOrder, bytesToRead)
	{
	}

	public override void StartNewRow()
	{
		base.StartNewRow();
		if (Numerics.Modulo8(base.BitsRead) != 0)
		{
			AdvancePosition();
		}
	}

	protected override void ReadEolBeforeFirstData()
	{
	}
}
