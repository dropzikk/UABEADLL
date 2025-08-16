namespace Avalonia.Automation;

public static class SelectionPatternIdentifiers
{
	public static AutomationProperty CanSelectMultipleProperty { get; } = new AutomationProperty();

	public static AutomationProperty IsSelectionRequiredProperty { get; } = new AutomationProperty();

	public static AutomationProperty SelectionProperty { get; } = new AutomationProperty();
}
