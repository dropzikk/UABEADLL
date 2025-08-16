using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[PrivateApi]
public interface IManagedPopupPositionerPopup
{
	IReadOnlyList<ManagedPopupPositionerScreenInfo> Screens { get; }

	Rect ParentClientAreaScreenGeometry { get; }

	double Scaling { get; }

	void MoveAndResize(Point devicePoint, Size virtualSize);
}
