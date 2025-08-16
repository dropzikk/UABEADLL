using Avalonia.Input;
using Avalonia.LogicalTree;

namespace Avalonia.Controls;

internal interface IMenuItem : IMenuElement, IInputElement, ILogical
{
	bool HasSubMenu { get; }

	bool IsPointerOverSubMenu { get; }

	bool IsSubMenuOpen { get; set; }

	bool StaysOpenOnClick { get; set; }

	bool IsTopLevel { get; }

	IMenuElement? Parent { get; }

	void RaiseClick();
}
