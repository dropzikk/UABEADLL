using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaEdit.CodeCompletion;

internal class PopupWithCustomPosition : Popup
{
	public Point Offset
	{
		get
		{
			return new Point(base.HorizontalOffset, base.VerticalOffset);
		}
		set
		{
			base.HorizontalOffset = value.X;
			base.VerticalOffset = value.Y;
		}
	}
}
