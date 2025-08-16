using Avalonia.Metadata;

namespace Avalonia.Animation.Easings;

[NotClientImplementable]
public interface IEasing
{
	double Ease(double progress);
}
