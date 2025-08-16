using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal struct Vp8Matrix
{
	private static readonly int[][] BiasMatrices = new int[3][]
	{
		new int[2] { 96, 110 },
		new int[2] { 96, 108 },
		new int[2] { 110, 115 }
	};

	private const int SharpenBits = 11;

	public unsafe fixed ushort Q[16];

	public unsafe fixed ushort IQ[16];

	public unsafe fixed uint Bias[16];

	public unsafe fixed uint ZThresh[16];

	public unsafe fixed short Sharpen[16];

	private static ReadOnlySpan<byte> FreqSharpening => new byte[16]
	{
		0, 30, 60, 90, 30, 60, 90, 90, 60, 90,
		90, 90, 90, 90, 90, 90
	};

	public unsafe int Expand(int type)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = ((i > 0) ? 1 : 0);
			int b = BiasMatrices[type][num];
			IQ[i] = (ushort)(131072 / Q[i]);
			Bias[i] = (uint)BIAS(b);
			ZThresh[i] = (131071 - Bias[i]) / IQ[i];
		}
		for (int i = 2; i < 16; i++)
		{
			Q[i] = Q[1];
			IQ[i] = IQ[1];
			Bias[i] = Bias[1];
			ZThresh[i] = ZThresh[1];
		}
		int num2 = 0;
		for (int i = 0; i < 16; i++)
		{
			if (type == 0)
			{
				Sharpen[i] = (short)(FreqSharpening[i] * Q[i] >> 11);
			}
			else
			{
				Sharpen[i] = 0;
			}
			num2 += Q[i];
		}
		return num2 + 8 >> 4;
	}

	private static int BIAS(int b)
	{
		return b << 9;
	}
}
