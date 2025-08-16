using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Formats.Png;

internal static class Adam7
{
	public static readonly int[] ColumnIncrement = new int[7] { 8, 8, 4, 4, 2, 2, 1 };

	public static readonly int[] FirstColumn = new int[7] { 0, 4, 0, 2, 0, 1, 0 };

	public static readonly int[] FirstRow = new int[7] { 0, 0, 4, 0, 2, 0, 1 };

	public static readonly int[] RowIncrement = new int[7] { 8, 8, 8, 4, 4, 2, 2 };

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeBlockWidth(int width, int pass)
	{
		return (width + ColumnIncrement[pass] - 1 - FirstColumn[pass]) / ColumnIncrement[pass];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeBlockHeight(int height, int pass)
	{
		return (height + RowIncrement[pass] - 1 - FirstRow[pass]) / RowIncrement[pass];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ComputeColumns(int width, int passIndex)
	{
		return passIndex switch
		{
			0 => (int)((uint)(width + 7) / 8u), 
			1 => (int)((uint)(width + 3) / 8u), 
			2 => (int)((uint)(width + 3) / 4u), 
			3 => (int)((uint)(width + 1) / 4u), 
			4 => (int)((uint)(width + 1) / 2u), 
			5 => (int)((uint)width / 2u), 
			6 => width, 
			_ => (int)Throw(passIndex), 
		};
		static uint Throw(int passIndex)
		{
			throw new ArgumentException($"Not a valid pass index: {passIndex}");
		}
	}
}
