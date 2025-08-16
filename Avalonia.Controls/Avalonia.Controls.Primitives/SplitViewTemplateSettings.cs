namespace Avalonia.Controls.Primitives;

public class SplitViewTemplateSettings : AvaloniaObject
{
	public static readonly StyledProperty<double> ClosedPaneWidthProperty = AvaloniaProperty.Register<SplitViewTemplateSettings, double>("ClosedPaneWidth", 0.0);

	public static readonly StyledProperty<GridLength> PaneColumnGridLengthProperty = AvaloniaProperty.Register<SplitViewTemplateSettings, GridLength>("PaneColumnGridLength");

	public double ClosedPaneWidth
	{
		get
		{
			return GetValue(ClosedPaneWidthProperty);
		}
		internal set
		{
			SetValue(ClosedPaneWidthProperty, value);
		}
	}

	public GridLength PaneColumnGridLength
	{
		get
		{
			return GetValue(PaneColumnGridLengthProperty);
		}
		internal set
		{
			SetValue(PaneColumnGridLengthProperty, value);
		}
	}

	internal SplitViewTemplateSettings()
	{
	}
}
