using System.Globalization;

namespace Avalonia.Media.TextFormatting;

public readonly record struct TextShaperOptions(IGlyphTypeface typeface, double fontRenderingEmSize = 12.0, sbyte bidiLevel = 0, CultureInfo? culture = null, double incrementalTabWidth = 0.0, double letterSpacing = 0.0);
