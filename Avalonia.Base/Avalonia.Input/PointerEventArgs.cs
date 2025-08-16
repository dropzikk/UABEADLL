using System;
using System.Collections.Generic;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace Avalonia.Input;

public class PointerEventArgs : RoutedEventArgs
{
	private readonly Visual? _rootVisual;

	private readonly Point _rootVisualPosition;

	private readonly PointerPointProperties _properties;

	private readonly Lazy<IReadOnlyList<RawPointerPoint>?>? _previousPoints;

	public IPointer Pointer { get; }

	public ulong Timestamp { get; }

	internal bool IsGestureRecognitionSkipped { get; private set; }

	public KeyModifiers KeyModifiers { get; }

	protected PointerPointProperties Properties => _properties;

	[Unstable("This constructor might be removed in 12.0. For unit testing, consider using IHeadlessWindow mouse methods.")]
	public PointerEventArgs(RoutedEvent routedEvent, object? source, IPointer pointer, Visual? rootVisual, Point rootVisualPosition, ulong timestamp, PointerPointProperties properties, KeyModifiers modifiers)
		: base(routedEvent)
	{
		base.Source = source;
		_rootVisual = rootVisual;
		_rootVisualPosition = rootVisualPosition;
		_properties = properties;
		Pointer = pointer;
		Timestamp = timestamp;
		KeyModifiers = modifiers;
	}

	internal PointerEventArgs(RoutedEvent routedEvent, object? source, IPointer pointer, Visual? rootVisual, Point rootVisualPosition, ulong timestamp, PointerPointProperties properties, KeyModifiers modifiers, Lazy<IReadOnlyList<RawPointerPoint>?>? previousPoints)
		: this(routedEvent, source, pointer, rootVisual, rootVisualPosition, timestamp, properties, modifiers)
	{
		_previousPoints = previousPoints;
	}

	private Point GetPosition(Point pt, Visual? relativeTo)
	{
		if (_rootVisual == null)
		{
			return default(Point);
		}
		if (relativeTo == null)
		{
			return pt;
		}
		if (_rootVisual != relativeTo.VisualRoot && relativeTo.VisualRoot != null)
		{
			PixelPoint point = _rootVisual.PointToScreen(pt);
			return relativeTo.PointToClient(point);
		}
		Matrix? matrix = _rootVisual.TransformToVisual(relativeTo);
		return (pt * matrix).GetValueOrDefault();
	}

	public Point GetPosition(Visual? relativeTo)
	{
		return GetPosition(_rootVisualPosition, relativeTo);
	}

	public PointerPoint GetCurrentPoint(Visual? relativeTo)
	{
		return new PointerPoint(Pointer, GetPosition(relativeTo), _properties);
	}

	public IReadOnlyList<PointerPoint> GetIntermediatePoints(Visual? relativeTo)
	{
		IReadOnlyList<RawPointerPoint> readOnlyList = _previousPoints?.Value;
		if (readOnlyList == null || readOnlyList.Count == 0)
		{
			return new PointerPoint[1] { GetCurrentPoint(relativeTo) };
		}
		PointerPoint[] array = new PointerPoint[readOnlyList.Count + 1];
		for (int i = 0; i < readOnlyList.Count; i++)
		{
			RawPointerPoint rawPoint = readOnlyList[i];
			array[i] = new PointerPoint(properties: new PointerPointProperties(_properties, rawPoint), pointer: Pointer, position: GetPosition(rawPoint.Position, relativeTo));
		}
		array[^1] = GetCurrentPoint(relativeTo);
		return array;
	}

	public void PreventGestureRecognition()
	{
		IsGestureRecognitionSkipped = true;
	}
}
