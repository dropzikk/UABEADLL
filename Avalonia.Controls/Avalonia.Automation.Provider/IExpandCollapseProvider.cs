namespace Avalonia.Automation.Provider;

public interface IExpandCollapseProvider
{
	ExpandCollapseState ExpandCollapseState { get; }

	bool ShowsMenu { get; }

	void Expand();

	void Collapse();
}
