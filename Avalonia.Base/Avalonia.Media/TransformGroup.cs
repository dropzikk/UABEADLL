using System;
using Avalonia.Collections;
using Avalonia.Metadata;

namespace Avalonia.Media;

public sealed class TransformGroup : Transform
{
	public static readonly StyledProperty<Transforms> ChildrenProperty = AvaloniaProperty.Register<TransformGroup, Transforms>("Children");

	[Content]
	public Transforms Children
	{
		get
		{
			return GetValue(ChildrenProperty);
		}
		set
		{
			SetValue(ChildrenProperty, value);
		}
	}

	public override Matrix Value
	{
		get
		{
			Matrix identity = Matrix.Identity;
			foreach (Transform child in Children)
			{
				identity *= child.Value;
			}
			return identity;
		}
	}

	public TransformGroup()
	{
		Children = new Transforms();
		Children.ResetBehavior = ResetBehavior.Remove;
		Children.CollectionChanged += delegate
		{
			Children.ForEachItem(delegate(Transform tr)
			{
				tr.Changed += ChildTransform_Changed;
			}, delegate(Transform tr)
			{
				tr.Changed -= ChildTransform_Changed;
			}, delegate
			{
			});
		};
	}

	private void ChildTransform_Changed(object? sender, EventArgs e)
	{
		RaiseChanged();
	}
}
