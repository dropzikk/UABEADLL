namespace Avalonia.Automation.Provider;

public interface IToggleProvider
{
	ToggleState ToggleState { get; }

	void Toggle();
}
