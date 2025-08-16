using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

internal static class OrderedDitherFactory
{
	public static DenseMatrix<uint> CreateDitherMatrix(uint length)
	{
		uint num = 0u;
		uint num2;
		do
		{
			num++;
			num2 = (uint)(1 << (int)num);
		}
		while (length > num2);
		DenseMatrix<uint> result = new DenseMatrix<uint>((int)length);
		uint num3 = 0u;
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				result[i, j] = Bayer(num3 / length, num3 % length, num);
				num3++;
			}
		}
		uint num4 = num2 * num2;
		uint num5 = 0u;
		for (uint num6 = 0u; num6 < num4; num6++)
		{
			bool flag = false;
			for (int k = 0; k < length; k++)
			{
				for (int l = 0; l < length; l++)
				{
					if (result[k, l] == num6)
					{
						result[k, l] -= num5;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				num5++;
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Bayer(uint x, uint y, uint order)
	{
		uint num = 0u;
		for (uint num2 = 0u; num2 < order; num2++)
		{
			uint num3 = (x & 1) ^ (y & 1);
			uint num4 = x & 1;
			num = (((num << 1) | num3) << 1) | num4;
			x >>= 1;
			y >>= 1;
		}
		return num;
	}
}
