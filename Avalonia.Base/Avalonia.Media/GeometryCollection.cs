using System;
using System.Collections.Generic;
using Avalonia.Collections;

namespace Avalonia.Media;

public sealed class GeometryCollection : AvaloniaList<Geometry>
{
	public GeometryGroup? Parent { get; set; }

	public GeometryCollection()
	{
		base.ResetBehavior = ResetBehavior.Remove;
		((IAvaloniaReadOnlyList<Geometry>)this).ForEachItem((Action<Geometry>)delegate
		{
			Parent?.Invalidate();
		}, (Action<Geometry>)delegate
		{
			Parent?.Invalidate();
		}, (Action)delegate
		{
			throw new NotSupportedException();
		}, weakSubscription: false);
	}

	public GeometryCollection(IEnumerable<Geometry> items)
		: base(items)
	{
		base.ResetBehavior = ResetBehavior.Remove;
		((IAvaloniaReadOnlyList<Geometry>)this).ForEachItem((Action<Geometry>)delegate
		{
			Parent?.Invalidate();
		}, (Action<Geometry>)delegate
		{
			Parent?.Invalidate();
		}, (Action)delegate
		{
			throw new NotSupportedException();
		}, weakSubscription: false);
	}
}
