using Avalonia.Metadata;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[PrivateApi]
public class ManagedPopupPositionerScreenInfo
{
	public Rect Bounds { get; }

	public Rect WorkingArea { get; }

	public ManagedPopupPositionerScreenInfo(Rect bounds, Rect workingArea)
	{
		Bounds = bounds;
		WorkingArea = workingArea;
	}
}
