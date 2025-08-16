using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Metadata;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[PrivateApi]
public class ManagedPopupPositioner : IPopupPositioner
{
	private readonly IManagedPopupPositionerPopup _popup;

	public ManagedPopupPositioner(IManagedPopupPositionerPopup popup)
	{
		_popup = popup;
	}

	private static Point GetAnchorPoint(Rect anchorRect, PopupAnchor edge)
	{
		double x = (edge.HasAllFlags(PopupAnchor.Left) ? anchorRect.X : ((!edge.HasAllFlags(PopupAnchor.Right)) ? (anchorRect.X + anchorRect.Width / 2.0) : anchorRect.Right));
		double y = (edge.HasAllFlags(PopupAnchor.Top) ? anchorRect.Y : ((!edge.HasAllFlags(PopupAnchor.Bottom)) ? (anchorRect.Y + anchorRect.Height / 2.0) : anchorRect.Bottom));
		return new Point(x, y);
	}

	private static Point Gravitate(Point anchorPoint, Size size, PopupGravity gravity)
	{
		double x = (gravity.HasAllFlags(PopupGravity.Left) ? (0.0 - size.Width) : ((!gravity.HasAllFlags(PopupGravity.Right)) ? ((0.0 - size.Width) / 2.0) : 0.0));
		double y = (gravity.HasAllFlags(PopupGravity.Top) ? (0.0 - size.Height) : ((!gravity.HasAllFlags(PopupGravity.Bottom)) ? ((0.0 - size.Height) / 2.0) : 0.0));
		return anchorPoint + new Point(x, y);
	}

	public void Update(PopupPositionerParameters parameters)
	{
		Rect rect = Calculate(parameters.Size * _popup.Scaling, new Rect(parameters.AnchorRectangle.TopLeft * _popup.Scaling, parameters.AnchorRectangle.Size * _popup.Scaling), parameters.Anchor, parameters.Gravity, parameters.ConstraintAdjustment, parameters.Offset * _popup.Scaling);
		_popup.MoveAndResize(rect.Position, rect.Size / _popup.Scaling);
	}

	private Rect Calculate(Size translatedSize, Rect anchorRect, PopupAnchor anchor, PopupGravity gravity, PopupPositionerConstraintAdjustment constraintAdjustment, Point offset)
	{
		Rect parentGeometry = _popup.ParentClientAreaScreenGeometry;
		anchorRect = anchorRect.Translate(parentGeometry.TopLeft);
		Rect bounds = GetBounds();
		Rect rect = GetUnconstrained(anchor, gravity);
		if (!FitsInBounds(rect, PopupAnchor.HorizontalMask) && constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.FlipX))
		{
			Rect rc2 = GetUnconstrained(anchor.FlipX(), gravity.FlipX());
			if (FitsInBounds(rc2, PopupAnchor.HorizontalMask))
			{
				rect = rect.WithX(rc2.X);
			}
		}
		if (constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.SlideX))
		{
			rect = rect.WithX(Math.Max(rect.X, bounds.X));
			if (rect.Right > bounds.Right)
			{
				rect = rect.WithX(bounds.Right - rect.Width);
			}
		}
		if (constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.ResizeX))
		{
			Rect rc3 = rect;
			if (!FitsInBounds(rc3, PopupAnchor.Left))
			{
				rc3 = rc3.WithX(bounds.X);
			}
			if (!FitsInBounds(rc3, PopupAnchor.Right))
			{
				rc3 = rc3.WithWidth(bounds.Width - rc3.X);
			}
			if (IsValid(in rc3))
			{
				rect = rc3;
			}
		}
		if (!FitsInBounds(rect, PopupAnchor.VerticalMask) && constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.FlipY))
		{
			Rect rc4 = GetUnconstrained(anchor.FlipY(), gravity.FlipY());
			if (FitsInBounds(rc4, PopupAnchor.VerticalMask))
			{
				rect = rect.WithY(rc4.Y);
			}
		}
		if (constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.SlideY))
		{
			rect = rect.WithY(Math.Max(rect.Y, bounds.Y));
			if (rect.Bottom > bounds.Bottom)
			{
				rect = rect.WithY(bounds.Bottom - rect.Height);
			}
		}
		if (constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.ResizeX))
		{
			Rect rc5 = rect;
			if (!FitsInBounds(rc5, PopupAnchor.Top))
			{
				rc5 = rc5.WithY(bounds.Y);
			}
			if (!FitsInBounds(rc5, PopupAnchor.Bottom))
			{
				rc5 = rc5.WithHeight(bounds.Bottom - rc5.Y);
			}
			if (IsValid(in rc5))
			{
				rect = rc5;
			}
		}
		return rect;
		bool FitsInBounds(Rect rc, PopupAnchor edge = PopupAnchor.AllMask)
		{
			if ((edge.HasAllFlags(PopupAnchor.Left) && rc.X < bounds.X) || (edge.HasAllFlags(PopupAnchor.Top) && rc.Y < bounds.Y) || (edge.HasAllFlags(PopupAnchor.Right) && rc.Right > bounds.Right) || (edge.HasAllFlags(PopupAnchor.Bottom) && rc.Bottom > bounds.Bottom))
			{
				return false;
			}
			return true;
		}
		Rect GetBounds()
		{
			IReadOnlyList<ManagedPopupPositionerScreenInfo> screens = _popup.Screens;
			ManagedPopupPositionerScreenInfo managedPopupPositionerScreenInfo = screens.FirstOrDefault((ManagedPopupPositionerScreenInfo s) => s.Bounds.ContainsExclusive(anchorRect.TopLeft)) ?? screens.FirstOrDefault((ManagedPopupPositionerScreenInfo s) => s.Bounds.Intersects(anchorRect)) ?? screens.FirstOrDefault((ManagedPopupPositionerScreenInfo s) => s.Bounds.ContainsExclusive(parentGeometry.TopLeft)) ?? screens.FirstOrDefault((ManagedPopupPositionerScreenInfo s) => s.Bounds.Intersects(parentGeometry)) ?? screens.FirstOrDefault();
			if (managedPopupPositionerScreenInfo != null && managedPopupPositionerScreenInfo.WorkingArea.Width == 0.0 && managedPopupPositionerScreenInfo.WorkingArea.Height == 0.0)
			{
				return managedPopupPositionerScreenInfo.Bounds;
			}
			return managedPopupPositionerScreenInfo?.WorkingArea ?? new Rect(0.0, 0.0, double.MaxValue, double.MaxValue);
		}
		Rect GetUnconstrained(PopupAnchor a, PopupGravity g)
		{
			return new Rect(Gravitate(GetAnchorPoint(anchorRect, a), translatedSize, g) + offset, translatedSize);
		}
		static bool IsValid(in Rect rc)
		{
			if (rc.Width > 0.0)
			{
				return rc.Height > 0.0;
			}
			return false;
		}
	}
}
