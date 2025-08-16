using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Media;

public class GeometryGroup : Geometry
{
	public static readonly DirectProperty<GeometryGroup, GeometryCollection> ChildrenProperty = AvaloniaProperty.RegisterDirect("Children", (GeometryGroup o) => o.Children, delegate(GeometryGroup o, GeometryCollection v)
	{
		o.Children = v;
	});

	public static readonly StyledProperty<FillRule> FillRuleProperty = AvaloniaProperty.Register<GeometryGroup, FillRule>("FillRule", FillRule.EvenOdd);

	private GeometryCollection _children;

	[Content]
	public GeometryCollection Children
	{
		get
		{
			return _children;
		}
		set
		{
			OnChildrenChanged(_children, value);
			SetAndRaise(ChildrenProperty, ref _children, value);
		}
	}

	public FillRule FillRule
	{
		get
		{
			return GetValue(FillRuleProperty);
		}
		set
		{
			SetValue(FillRuleProperty, value);
		}
	}

	public GeometryGroup()
	{
		_children = new GeometryCollection
		{
			Parent = this
		};
	}

	public override Geometry Clone()
	{
		GeometryGroup geometryGroup = new GeometryGroup
		{
			FillRule = FillRule,
			Transform = base.Transform
		};
		if (_children.Count > 0)
		{
			geometryGroup.Children = new GeometryCollection(_children);
		}
		return geometryGroup;
	}

	protected void OnChildrenChanged(GeometryCollection oldChildren, GeometryCollection newChildren)
	{
		oldChildren.Parent = null;
		newChildren.Parent = this;
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		if (_children.Count > 0)
		{
			IPlatformRenderInterface requiredService = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
			IGeometryImpl[] array = new IGeometryImpl[_children.Count];
			for (int i = 0; i < _children.Count; i++)
			{
				array[i] = _children[i].PlatformImpl;
			}
			return requiredService.CreateGeometryGroup(FillRule, array);
		}
		return null;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		string name = change.Property.Name;
		if (name == "FillRule" || name == "Children")
		{
			InvalidateGeometry();
		}
	}

	internal void Invalidate()
	{
		InvalidateGeometry();
	}
}
