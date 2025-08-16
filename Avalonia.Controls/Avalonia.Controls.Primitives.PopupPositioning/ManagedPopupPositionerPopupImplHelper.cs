using System.Collections.Generic;
using System.Linq;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[PrivateApi]
public class ManagedPopupPositionerPopupImplHelper : IManagedPopupPositionerPopup
{
	public delegate void MoveResizeDelegate(PixelPoint position, Size size, double scaling);

	private readonly IWindowBaseImpl _parent;

	private readonly MoveResizeDelegate _moveResize;

	public IReadOnlyList<ManagedPopupPositionerScreenInfo> Screens => _parent.Screen.AllScreens.Select((Screen s) => new ManagedPopupPositionerScreenInfo(s.Bounds.ToRect(1.0), s.WorkingArea.ToRect(1.0))).ToArray();

	public Rect ParentClientAreaScreenGeometry
	{
		get
		{
			PixelPoint pixelPoint = _parent.PointToScreen(default(Point));
			Size size = _parent.ClientSize * Scaling;
			return new Rect(pixelPoint.X, pixelPoint.Y, size.Width, size.Height);
		}
	}

	public virtual double Scaling => _parent.DesktopScaling;

	public ManagedPopupPositionerPopupImplHelper(IWindowBaseImpl parent, MoveResizeDelegate moveResize)
	{
		_parent = parent;
		_moveResize = moveResize;
	}

	public void MoveAndResize(Point devicePoint, Size virtualSize)
	{
		_moveResize(new PixelPoint((int)devicePoint.X, (int)devicePoint.Y), virtualSize, _parent.RenderScaling);
	}
}
