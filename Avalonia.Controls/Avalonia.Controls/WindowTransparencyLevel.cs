using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Controls;

public readonly record struct WindowTransparencyLevel(string value)
{
	public static WindowTransparencyLevel None { get; } = new WindowTransparencyLevel("None");

	public static WindowTransparencyLevel Transparent { get; } = new WindowTransparencyLevel("Transparent");

	public static WindowTransparencyLevel Blur { get; } = new WindowTransparencyLevel("Blur");

	public static WindowTransparencyLevel AcrylicBlur { get; } = new WindowTransparencyLevel("AcrylicBlur");

	public static WindowTransparencyLevel Mica { get; } = new WindowTransparencyLevel("Mica");

	private readonly string _value = value;

	public override string ToString()
	{
		return value;
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		return false;
	}
}
