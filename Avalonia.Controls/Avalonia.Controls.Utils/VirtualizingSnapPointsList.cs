using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace Avalonia.Controls.Utils;

internal class VirtualizingSnapPointsList : IReadOnlyList<double>, IEnumerable<double>, IEnumerable, IReadOnlyCollection<double>
{
	private const int ExtraCount = 2;

	private readonly RealizedStackElements _realizedElements;

	private readonly Orientation _orientation;

	private readonly Orientation _parentOrientation;

	private readonly SnapPointsAlignment _snapPointsAlignment;

	private readonly double _size;

	private readonly int _start = -1;

	private readonly int _end;

	public double this[int index]
	{
		get
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			index += _start;
			double num = 0.0;
			double size = _size;
			switch (_orientation)
			{
			case Orientation.Horizontal:
			{
				Control element = _realizedElements.GetElement(index);
				if (element != null)
				{
					switch (_snapPointsAlignment)
					{
					case SnapPointsAlignment.Near:
						num = element.Bounds.Left;
						break;
					case SnapPointsAlignment.Center:
						num = element.Bounds.Center.X;
						break;
					case SnapPointsAlignment.Far:
						num = element.Bounds.Right;
						break;
					}
					break;
				}
				int num3 = index;
				if (index > _realizedElements.LastIndex)
				{
					num3 -= _realizedElements.LastIndex + 1;
				}
				num = (double)num3 * size;
				switch (_snapPointsAlignment)
				{
				case SnapPointsAlignment.Center:
					num += size / 2.0;
					break;
				case SnapPointsAlignment.Far:
					num += size;
					break;
				}
				if (index > _realizedElements.LastIndex)
				{
					Control element3 = _realizedElements.GetElement(_realizedElements.LastIndex);
					if (element3 != null)
					{
						num += element3.Bounds.Right;
					}
				}
				break;
			}
			case Orientation.Vertical:
			{
				Control element = _realizedElements.GetElement(index);
				if (element != null)
				{
					switch (_snapPointsAlignment)
					{
					case SnapPointsAlignment.Near:
						num = element.Bounds.Top;
						break;
					case SnapPointsAlignment.Center:
						num = element.Bounds.Center.Y;
						break;
					case SnapPointsAlignment.Far:
						num = element.Bounds.Bottom;
						break;
					}
					break;
				}
				int num2 = index;
				if (index > _realizedElements.LastIndex)
				{
					num2 -= _realizedElements.LastIndex + 1;
				}
				num = (double)num2 * size;
				switch (_snapPointsAlignment)
				{
				case SnapPointsAlignment.Center:
					num += size / 2.0;
					break;
				case SnapPointsAlignment.Far:
					num += size;
					break;
				}
				if (index > _realizedElements.LastIndex)
				{
					Control element2 = _realizedElements.GetElement(_realizedElements.LastIndex);
					if (element2 != null)
					{
						num += element2.Bounds.Bottom;
					}
				}
				break;
			}
			}
			return num;
		}
	}

	public int Count
	{
		get
		{
			if (_parentOrientation == _orientation)
			{
				return _end - _start + 1;
			}
			return 0;
		}
	}

	public VirtualizingSnapPointsList(RealizedStackElements realizedElements, int count, Orientation orientation, Orientation parentOrientation, SnapPointsAlignment snapPointsAlignment, double size)
	{
		_realizedElements = realizedElements;
		_orientation = orientation;
		_parentOrientation = parentOrientation;
		_snapPointsAlignment = snapPointsAlignment;
		_size = size;
		if (parentOrientation == orientation)
		{
			_start = Math.Max(0, _realizedElements.FirstIndex - 2);
			_end = Math.Min(count - 1, _realizedElements.LastIndex + 2);
		}
	}

	public IEnumerator<double> GetEnumerator()
	{
		for (int i = 0; i < Count; i++)
		{
			yield return this[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
