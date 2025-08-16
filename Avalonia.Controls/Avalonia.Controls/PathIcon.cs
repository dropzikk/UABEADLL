using Avalonia.Media;

namespace Avalonia.Controls;

public class PathIcon : IconElement
{
	public static readonly StyledProperty<Geometry> DataProperty;

	public Geometry Data
	{
		get
		{
			return GetValue(DataProperty);
		}
		set
		{
			SetValue(DataProperty, value);
		}
	}

	static PathIcon()
	{
		DataProperty = AvaloniaProperty.Register<PathIcon, Geometry>("Data");
		Visual.AffectsRender<PathIcon>(new AvaloniaProperty[1] { DataProperty });
	}
}
