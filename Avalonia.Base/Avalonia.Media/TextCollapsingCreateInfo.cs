using Avalonia.Media.TextFormatting;

namespace Avalonia.Media;

public readonly record struct TextCollapsingCreateInfo(double width, TextRunProperties textRunProperties, FlowDirection flowDirection)
{
	public readonly double Width = width;

	public readonly TextRunProperties TextRunProperties = textRunProperties;

	public readonly FlowDirection FlowDirection = flowDirection;
}
