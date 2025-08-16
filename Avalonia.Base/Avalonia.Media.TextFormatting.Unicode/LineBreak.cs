using System.Diagnostics;

namespace Avalonia.Media.TextFormatting.Unicode;

[DebuggerDisplay("{PositionMeasure}/{PositionWrap} @ {Required}")]
public readonly record struct LineBreak(int positionMeasure, int positionWrap, bool required = false);
