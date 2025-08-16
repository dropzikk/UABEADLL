using Avalonia.Metadata;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[NotClientImplementable]
public interface IPopupPositioner
{
	void Update(PopupPositionerParameters parameters);
}
