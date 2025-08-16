using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IMutableExperimentalAcrylicMaterial : IExperimentalAcrylicMaterial
{
	IExperimentalAcrylicMaterial ToImmutable();
}
