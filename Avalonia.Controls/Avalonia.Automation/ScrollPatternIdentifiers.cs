namespace Avalonia.Automation;

public static class ScrollPatternIdentifiers
{
	public const double NoScroll = -1.0;

	public static AutomationProperty HorizontallyScrollableProperty { get; } = new AutomationProperty();

	public static AutomationProperty HorizontalScrollPercentProperty { get; } = new AutomationProperty();

	public static AutomationProperty HorizontalViewSizeProperty { get; } = new AutomationProperty();

	public static AutomationProperty VerticallyScrollableProperty { get; } = new AutomationProperty();

	public static AutomationProperty VerticalScrollPercentProperty { get; } = new AutomationProperty();

	public static AutomationProperty VerticalViewSizeProperty { get; } = new AutomationProperty();
}
