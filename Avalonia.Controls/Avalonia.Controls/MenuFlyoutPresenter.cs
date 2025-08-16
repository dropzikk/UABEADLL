using System;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace Avalonia.Controls;

public class MenuFlyoutPresenter : MenuBase
{
	public MenuFlyoutPresenter()
		: base(new DefaultMenuInteractionHandler(isContextMenu: true))
	{
	}

	public MenuFlyoutPresenter(IMenuInteractionHandler menuInteractionHandler)
		: base(menuInteractionHandler)
	{
	}

	public override void Close()
	{
		Popup popup = this.FindLogicalAncestorOfType<Popup>();
		if (popup != null)
		{
			base.SelectedIndex = -1;
			popup.IsOpen = false;
		}
	}

	public override void Open()
	{
		throw new NotSupportedException("Use MenuFlyout.ShowAt(Control) instead");
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		foreach (ILogical logicalChild in base.LogicalChildren)
		{
			if (logicalChild is MenuItem menuItem)
			{
				menuItem.IsSubMenuOpen = false;
			}
		}
	}
}
