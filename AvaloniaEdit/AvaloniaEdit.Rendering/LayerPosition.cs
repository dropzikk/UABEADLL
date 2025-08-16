using System;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaEdit.Rendering;

internal sealed class LayerPosition : IComparable<LayerPosition>
{
	internal static readonly AttachedProperty<LayerPosition> LayerPositionProperty = AvaloniaProperty.RegisterAttached<LayerPosition, Control, LayerPosition>("LayerPosition");

	internal readonly KnownLayer KnownLayer;

	internal readonly LayerInsertionPosition Position;

	public static void SetLayerPosition(Control layer, LayerPosition value)
	{
		layer.SetValue(LayerPositionProperty, value);
	}

	public static LayerPosition GetLayerPosition(Control layer)
	{
		return layer.GetValue(LayerPositionProperty);
	}

	public LayerPosition(KnownLayer knownLayer, LayerInsertionPosition position)
	{
		KnownLayer = knownLayer;
		Position = position;
	}

	public int CompareTo(LayerPosition other)
	{
		int num = KnownLayer.CompareTo(other.KnownLayer);
		if (num == 0)
		{
			return Position.CompareTo(other.Position);
		}
		return num;
	}
}
