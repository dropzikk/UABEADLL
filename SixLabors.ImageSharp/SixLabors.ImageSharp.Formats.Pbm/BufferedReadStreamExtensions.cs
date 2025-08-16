using System.IO;
using SixLabors.ImageSharp.IO;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal static class BufferedReadStreamExtensions
{
	public static bool SkipWhitespaceAndComments(this BufferedReadStream stream)
	{
		int num;
		do
		{
			num = stream.ReadByte();
			if (num < 0)
			{
				return false;
			}
			if (num != 35)
			{
				continue;
			}
			int num2;
			do
			{
				num2 = stream.ReadByte();
				if (num2 < 0)
				{
					return false;
				}
			}
			while (num2 != 10);
			num = num2;
		}
		while (((uint)(num - 9) <= 1u || num == 13 || num == 32) ? true : false);
		stream.Seek(-1L, SeekOrigin.Current);
		return true;
	}

	public static bool ReadDecimal(this BufferedReadStream stream, out int value)
	{
		value = 0;
		while (true)
		{
			int num = stream.ReadByte();
			if (num < 0)
			{
				return false;
			}
			num -= 48;
			if ((uint)num > 9u)
			{
				break;
			}
			value = value * 10 + num;
		}
		return true;
	}
}
