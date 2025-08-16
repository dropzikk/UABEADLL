using System;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[Unstable]
internal static class PopupPositionerExtensions
{
	public static void ConfigurePosition(this ref PopupPositionerParameters positionerParameters, TopLevel topLevel, Visual target, PlacementMode placement, Point offset, PopupAnchor anchor, PopupGravity gravity, PopupPositionerConstraintAdjustment constraintAdjustment, Rect? rect, FlowDirection flowDirection)
	{
		positionerParameters.Offset = offset;
		positionerParameters.ConstraintAdjustment = constraintAdjustment;
		if (placement == PlacementMode.Pointer)
		{
			Point position = topLevel.PointToClient(topLevel.LastPointerPosition.GetValueOrDefault());
			positionerParameters.AnchorRectangle = new Rect(position, new Size(1.0, 1.0));
			positionerParameters.Anchor = PopupAnchor.TopLeft;
			positionerParameters.Gravity = PopupGravity.BottomRight;
		}
		else
		{
			if (target == null)
			{
				throw new InvalidOperationException("Placement mode is not Pointer and PlacementTarget is null");
			}
			Matrix? matrix = target.TransformToVisual(topLevel);
			if (!matrix.HasValue)
			{
				if (target.GetVisualRoot() == null)
				{
					throw new InvalidOperationException("Target control is not attached to the visual tree");
				}
				throw new InvalidOperationException("Target control is not in the same tree as the popup parent");
			}
			Rect rect2 = new Rect(default(Point), target.Bounds.Size);
			positionerParameters.AnchorRectangle = (rect ?? rect2).Intersect(rect2).TransformToAABB(matrix.Value);
			(positionerParameters.Anchor, positionerParameters.Gravity) = placement switch
			{
				PlacementMode.Bottom => (PopupAnchor.Bottom, PopupGravity.Bottom), 
				PlacementMode.Right => (PopupAnchor.Right, PopupGravity.Right), 
				PlacementMode.Left => (PopupAnchor.Left, PopupGravity.Left), 
				PlacementMode.Top => (PopupAnchor.Top, PopupGravity.Top), 
				PlacementMode.Center => (PopupAnchor.None, PopupGravity.None), 
				PlacementMode.AnchorAndGravity => (anchor, gravity), 
				PlacementMode.TopEdgeAlignedRight => (PopupAnchor.TopRight, PopupGravity.TopLeft), 
				PlacementMode.TopEdgeAlignedLeft => (PopupAnchor.TopLeft, PopupGravity.TopRight), 
				PlacementMode.BottomEdgeAlignedLeft => (PopupAnchor.BottomLeft, PopupGravity.BottomRight), 
				PlacementMode.BottomEdgeAlignedRight => (PopupAnchor.BottomRight, PopupGravity.BottomLeft), 
				PlacementMode.LeftEdgeAlignedTop => (PopupAnchor.TopLeft, PopupGravity.BottomLeft), 
				PlacementMode.LeftEdgeAlignedBottom => (PopupAnchor.BottomLeft, PopupGravity.TopLeft), 
				PlacementMode.RightEdgeAlignedTop => (PopupAnchor.TopRight, PopupGravity.BottomRight), 
				PlacementMode.RightEdgeAlignedBottom => (PopupAnchor.BottomRight, PopupGravity.TopRight), 
				_ => throw new ArgumentOutOfRangeException("placement", placement, "Invalid value for Popup.PlacementMode"), 
			};
		}
		if (flowDirection == FlowDirection.RightToLeft)
		{
			if ((positionerParameters.Anchor & PopupAnchor.Right) == PopupAnchor.Right)
			{
				positionerParameters.Anchor ^= PopupAnchor.Right;
				positionerParameters.Anchor |= PopupAnchor.Left;
			}
			else if ((positionerParameters.Anchor & PopupAnchor.Left) == PopupAnchor.Left)
			{
				positionerParameters.Anchor ^= PopupAnchor.Left;
				positionerParameters.Anchor |= PopupAnchor.Right;
			}
			if ((positionerParameters.Gravity & PopupGravity.Right) == PopupGravity.Right)
			{
				positionerParameters.Gravity ^= PopupGravity.Right;
				positionerParameters.Gravity |= PopupGravity.Left;
			}
			else if ((positionerParameters.Gravity & PopupGravity.Left) == PopupGravity.Left)
			{
				positionerParameters.Gravity ^= PopupGravity.Left;
				positionerParameters.Gravity |= PopupGravity.Right;
			}
		}
	}
}
