namespace Avalonia.Automation.Provider;

public interface ISelectionItemProvider
{
	bool IsSelected { get; }

	ISelectionProvider? SelectionContainer { get; }

	void AddToSelection();

	void RemoveFromSelection();

	void Select();
}
