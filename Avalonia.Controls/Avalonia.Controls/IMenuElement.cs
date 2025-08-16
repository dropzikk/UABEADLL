using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace Avalonia.Controls;

internal interface IMenuElement : IInputElement, ILogical
{
	IMenuItem? SelectedItem { get; set; }

	IEnumerable<IMenuItem> SubItems { get; }

	void Open();

	void Close();

	bool MoveSelection(NavigationDirection direction, bool wrap);
}
