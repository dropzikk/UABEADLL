using Avalonia.Media;

namespace Avalonia.Controls;

public interface IColorPalette
{
	int ColorCount { get; }

	int ShadeCount { get; }

	Color GetColor(int colorIndex, int shadeIndex);
}
