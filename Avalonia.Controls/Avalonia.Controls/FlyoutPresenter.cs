using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace Avalonia.Controls;

public class FlyoutPresenter : ContentControl
{
	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
		{
			Popup popup = this.FindLogicalAncestorOfType<Popup>();
			if (popup != null)
			{
				popup.IsOpen = false;
				e.Handled = true;
			}
		}
		base.OnKeyDown(e);
	}
}
