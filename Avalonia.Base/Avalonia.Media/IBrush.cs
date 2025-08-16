using System.ComponentModel;
using Avalonia.Metadata;

namespace Avalonia.Media;

[TypeConverter(typeof(BrushConverter))]
[NotClientImplementable]
public interface IBrush
{
	double Opacity { get; }

	ITransform? Transform { get; }

	RelativePoint TransformOrigin { get; }
}
