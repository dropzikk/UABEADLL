using System;
using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Input.Raw;

[PrivateApi]
public class RawPointerEventArgs : RawInputEventArgs
{
	private RawPointerPoint _point;

	public long RawPointerId { get; set; }

	public RawPointerPoint Point
	{
		get
		{
			return _point;
		}
		set
		{
			_point = value;
		}
	}

	public Point Position
	{
		get
		{
			return _point.Position;
		}
		set
		{
			_point.Position = value;
		}
	}

	public RawPointerEventType Type { get; set; }

	public RawInputModifiers InputModifiers { get; set; }

	public Lazy<IReadOnlyList<RawPointerPoint>?>? IntermediatePoints { get; set; }

	internal IInputElement? InputHitTestResult { get; set; }

	public RawPointerEventArgs(IInputDevice device, ulong timestamp, IInputRoot root, RawPointerEventType type, Point position, RawInputModifiers inputModifiers)
		: base(device, timestamp, root)
	{
		Point = new RawPointerPoint();
		Position = position;
		Type = type;
		InputModifiers = inputModifiers;
	}

	public RawPointerEventArgs(IInputDevice device, ulong timestamp, IInputRoot root, RawPointerEventType type, RawPointerPoint point, RawInputModifiers inputModifiers)
		: base(device, timestamp, root)
	{
		Point = point;
		Type = type;
		InputModifiers = inputModifiers;
	}
}
