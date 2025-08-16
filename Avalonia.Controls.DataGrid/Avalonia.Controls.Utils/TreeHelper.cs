using Avalonia.Input;
using Avalonia.VisualTree;

namespace Avalonia.Controls.Utils;

internal static class TreeHelper
{
	internal static bool ContainsChild(this Visual element, Visual child)
	{
		if (element != null)
		{
			while (child != null)
			{
				if (child == element)
				{
					return true;
				}
				Visual visualParent = child.GetVisualParent();
				if (visualParent == null && child is Control control)
				{
					visualParent = control.VisualParent;
				}
				child = visualParent;
			}
		}
		return false;
	}

	internal static bool ContainsFocusedElement(this Visual element)
	{
		if (element is InputElement inputElement)
		{
			return inputElement.IsKeyboardFocusWithin;
		}
		return false;
	}
}
