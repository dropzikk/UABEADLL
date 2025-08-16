using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class SixteenColorPalette : IColorPalette
{
	private static Color[,] colorChart = new Color[8, 2]
	{
		{
			Colors.White,
			Colors.Silver
		},
		{
			Colors.Gray,
			Colors.Black
		},
		{
			Colors.Red,
			Colors.Maroon
		},
		{
			Colors.Yellow,
			Colors.Olive
		},
		{
			Colors.Lime,
			Colors.Green
		},
		{
			Colors.Aqua,
			Colors.Teal
		},
		{
			Colors.Blue,
			Colors.Navy
		},
		{
			Colors.Fuchsia,
			Colors.Purple
		}
	};

	public int ColorCount => colorChart.GetLength(0);

	public int ShadeCount => colorChart.GetLength(1);

	public Color GetColor(int colorIndex, int shadeIndex)
	{
		return colorChart[MathUtilities.Clamp(colorIndex, 0, colorChart.GetLength(0) - 1), MathUtilities.Clamp(shadeIndex, 0, colorChart.GetLength(1) - 1)];
	}
}
