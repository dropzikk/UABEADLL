using System;
using System.IO;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class T4BitCompressor : TiffCcittCompressor
{
	private readonly bool useModifiedHuffman;

	public override TiffCompression Method
	{
		get
		{
			if (!useModifiedHuffman)
			{
				return TiffCompression.CcittGroup3Fax;
			}
			return TiffCompression.Ccitt1D;
		}
	}

	public T4BitCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel, bool useModifiedHuffman = false)
		: base(output, allocator, width, bitsPerPixel)
	{
		this.useModifiedHuffman = useModifiedHuffman;
	}

	protected override void CompressStrip(Span<byte> pixelsAsGray, int height, Span<byte> compressedData)
	{
		if (!useModifiedHuffman)
		{
			WriteCode(12u, 1u, compressedData);
		}
		for (int i = 0; i < height; i++)
		{
			bool flag = true;
			bool flag2 = true;
			int num = 0;
			Span<byte> span = pixelsAsGray.Slice(i * base.Width, base.Width);
			while (num < base.Width)
			{
				uint num2 = 0u;
				for (int j = num; j < base.Width && (!flag || span[j] == byte.MaxValue); j++)
				{
					if (flag && span[j] == byte.MaxValue)
					{
						num2++;
						continue;
					}
					if (!flag && span[j] != 0)
					{
						break;
					}
					if (!flag && span[j] == 0)
					{
						num2++;
					}
				}
				if (flag2 && num2 == 0)
				{
					WriteCode(8u, 53u, compressedData);
					flag = false;
					flag2 = false;
					continue;
				}
				uint codeLength;
				uint termCode;
				if (num2 <= 63)
				{
					termCode = TiffCcittCompressor.GetTermCode(num2, out codeLength, flag);
					WriteCode(codeLength, termCode, compressedData);
					num += (int)num2;
					flag2 = false;
					flag = !flag;
					continue;
				}
				num2 = TiffCcittCompressor.GetBestFittingMakeupRunLength(num2);
				termCode = TiffCcittCompressor.GetMakeupCode(num2, out codeLength, flag);
				WriteCode(codeLength, termCode, compressedData);
				num += (int)num2;
				if (num == base.Width)
				{
					if (flag)
					{
						WriteCode(8u, 53u, compressedData);
					}
					else
					{
						WriteCode(10u, 55u, compressedData);
					}
				}
			}
			WriteEndOfLine(compressedData);
		}
	}

	private void WriteEndOfLine(Span<byte> compressedData)
	{
		if (useModifiedHuffman)
		{
			PadByte();
		}
		else
		{
			WriteCode(12u, 1u, compressedData);
		}
	}
}
