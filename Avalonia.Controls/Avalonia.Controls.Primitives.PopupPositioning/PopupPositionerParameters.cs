using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Metadata;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[Unstable]
public record struct PopupPositionerParameters
{
	public Size Size { get; set; }

	public Rect AnchorRectangle { get; set; }

	public PopupAnchor Anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			value.ValidateEdge();
			_anchor = value;
		}
	}

	public PopupGravity Gravity
	{
		get
		{
			return _gravity;
		}
		set
		{
			value.ValidateGravity();
			_gravity = value;
		}
	}

	public PopupPositionerConstraintAdjustment ConstraintAdjustment { get; set; }

	public Point Offset { get; set; }

	private PopupGravity _gravity;

	private PopupAnchor _anchor;

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("Size = ");
		builder.Append(Size.ToString());
		builder.Append(", AnchorRectangle = ");
		builder.Append(AnchorRectangle.ToString());
		builder.Append(", Anchor = ");
		builder.Append(Anchor.ToString());
		builder.Append(", Gravity = ");
		builder.Append(Gravity.ToString());
		builder.Append(", ConstraintAdjustment = ");
		builder.Append(ConstraintAdjustment.ToString());
		builder.Append(", Offset = ");
		builder.Append(Offset.ToString());
		return true;
	}

	[CompilerGenerated]
	public override readonly int GetHashCode()
	{
		return ((((EqualityComparer<PopupGravity>.Default.GetHashCode(_gravity) * -1521134295 + EqualityComparer<PopupAnchor>.Default.GetHashCode(_anchor)) * -1521134295 + EqualityComparer<Size>.Default.GetHashCode(Size)) * -1521134295 + EqualityComparer<Rect>.Default.GetHashCode(AnchorRectangle)) * -1521134295 + EqualityComparer<PopupPositionerConstraintAdjustment>.Default.GetHashCode(ConstraintAdjustment)) * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(Offset);
	}

	[CompilerGenerated]
	public readonly bool Equals(PopupPositionerParameters other)
	{
		if (EqualityComparer<PopupGravity>.Default.Equals(_gravity, other._gravity) && EqualityComparer<PopupAnchor>.Default.Equals(_anchor, other._anchor) && EqualityComparer<Size>.Default.Equals(Size, other.Size) && EqualityComparer<Rect>.Default.Equals(AnchorRectangle, other.AnchorRectangle) && EqualityComparer<PopupPositionerConstraintAdjustment>.Default.Equals(ConstraintAdjustment, other.ConstraintAdjustment))
		{
			return EqualityComparer<Point>.Default.Equals(Offset, other.Offset);
		}
		return false;
	}
}
