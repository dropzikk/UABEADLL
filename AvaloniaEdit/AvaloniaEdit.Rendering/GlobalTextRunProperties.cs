using System.Globalization;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaEdit.Rendering;

internal sealed class GlobalTextRunProperties : TextRunProperties
{
	internal Typeface typeface;

	internal double fontRenderingEmSize;

	internal IBrush? foregroundBrush;

	internal CultureInfo? cultureInfo;

	public override Typeface Typeface => typeface;

	public override double FontRenderingEmSize => fontRenderingEmSize;

	public override TextDecorationCollection? TextDecorations => null;

	public override IBrush? ForegroundBrush => foregroundBrush;

	public override IBrush? BackgroundBrush => null;

	public override CultureInfo? CultureInfo => cultureInfo;
}
