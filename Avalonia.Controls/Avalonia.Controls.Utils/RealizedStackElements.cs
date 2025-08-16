using System;
using System.Collections.Generic;
using Avalonia.Utilities;

namespace Avalonia.Controls.Utils;

internal class RealizedStackElements
{
	private int _firstIndex;

	private List<Control?>? _elements;

	private List<double>? _sizes;

	private double _startU;

	private bool _startUUnstable;

	public int Count => _elements?.Count ?? 0;

	public int FirstIndex
	{
		get
		{
			List<Control?>? elements = _elements;
			if (elements == null || elements.Count <= 0)
			{
				return -1;
			}
			return _firstIndex;
		}
	}

	public int LastIndex
	{
		get
		{
			List<Control?>? elements = _elements;
			if (elements == null || elements.Count <= 0)
			{
				return -1;
			}
			return _firstIndex + _elements.Count - 1;
		}
	}

	public IReadOnlyList<Control?> Elements => _elements ?? (_elements = new List<Control>());

	public IReadOnlyList<double> SizeU => _sizes ?? (_sizes = new List<double>());

	public double StartU => _startU;

	public void Add(int index, Control element, double u, double sizeU)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (_elements == null)
		{
			_elements = new List<Control>();
		}
		if (_sizes == null)
		{
			_sizes = new List<double>();
		}
		if (Count == 0)
		{
			_elements.Add(element);
			_sizes.Add(sizeU);
			_startU = u;
			_firstIndex = index;
			return;
		}
		if (index == LastIndex + 1)
		{
			_elements.Add(element);
			_sizes.Add(sizeU);
			return;
		}
		if (index == FirstIndex - 1)
		{
			_firstIndex--;
			_elements.Insert(0, element);
			_sizes.Insert(0, sizeU);
			_startU = u;
			return;
		}
		throw new NotSupportedException("Can only add items to the beginning or end of realized elements.");
	}

	public Control? GetElement(int index)
	{
		int num = index - FirstIndex;
		if (num >= 0 && num < _elements?.Count)
		{
			return _elements[num];
		}
		return null;
	}

	public (int index, double position) GetOrEstimateAnchorElementForViewport(double viewportStartU, double viewportEndU, int itemCount, ref double estimatedElementSizeU)
	{
		if (itemCount <= 0)
		{
			return (index: -1, position: 0.0);
		}
		if (MathUtilities.IsZero(viewportStartU))
		{
			return (index: 0, position: 0.0);
		}
		if (_sizes != null && !_startUUnstable)
		{
			double num = _startU;
			for (int i = 0; i < _sizes.Count; i++)
			{
				double num2 = _sizes[i];
				if (double.IsNaN(num2))
				{
					break;
				}
				double num3 = num + num2;
				if (num3 > viewportStartU && num < viewportEndU)
				{
					return (index: FirstIndex + i, position: num);
				}
				num = num3;
			}
		}
		double num4 = EstimateElementSizeU();
		double num5 = ((num4 != -1.0) ? num4 : estimatedElementSizeU);
		double num6 = (estimatedElementSizeU = num5);
		int num7 = Math.Min((int)(viewportStartU / num6), itemCount - 1);
		return (index: num7, position: (double)num7 * num6);
	}

	public double GetElementU(int index)
	{
		if (index < FirstIndex || _sizes == null)
		{
			return double.NaN;
		}
		int num = index - FirstIndex;
		if (num >= _sizes.Count)
		{
			return double.NaN;
		}
		double num2 = StartU;
		for (int i = 0; i < num; i++)
		{
			num2 += _sizes[i];
		}
		return num2;
	}

	public double GetOrEstimateElementU(int index, ref double estimatedElementSizeU)
	{
		double elementU = GetElementU(index);
		if (!double.IsNaN(elementU))
		{
			return elementU;
		}
		double num = EstimateElementSizeU();
		double num2 = ((num != -1.0) ? num : estimatedElementSizeU);
		return (double)index * (estimatedElementSizeU = num2);
	}

	public double EstimateElementSizeU()
	{
		double num = 0.0;
		double num2 = 0.0;
		if (_sizes != null)
		{
			foreach (double size in _sizes)
			{
				if (!double.IsNaN(size))
				{
					num += size;
					num2 += 1.0;
				}
			}
		}
		if (num2 == 0.0 || num == 0.0)
		{
			return -1.0;
		}
		return num / num2;
	}

	public int GetIndex(Control element)
	{
		int? num = _elements?.IndexOf(element);
		if (num.HasValue)
		{
			int valueOrDefault = num.GetValueOrDefault();
			if (valueOrDefault >= 0)
			{
				return valueOrDefault + FirstIndex;
			}
		}
		return -1;
	}

	public void ItemsInserted(int index, int count, Action<Control, int, int> updateElementIndex)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (_elements == null || _elements.Count == 0)
		{
			return;
		}
		int firstIndex = FirstIndex;
		int num = index - firstIndex;
		if (num >= Count)
		{
			return;
		}
		int count2 = _elements.Count;
		int num2 = Math.Max(num, 0);
		int num3 = num + count;
		for (int i = num2; i < count2; i++)
		{
			Control control = _elements[i];
			if (control != null)
			{
				updateElementIndex(control, num3 - count, num3);
			}
			num3++;
		}
		if (num < 0)
		{
			_firstIndex += count;
			return;
		}
		_elements.InsertMany(num, null, count);
		_sizes.InsertMany(num, double.NaN, count);
	}

	public void ItemsRemoved(int index, int count, Action<Control, int, int> updateElementIndex, Action<Control> recycleElement)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (_elements == null || _elements.Count == 0)
		{
			return;
		}
		int num = FirstIndex;
		int lastIndex = LastIndex;
		int num2 = index - num;
		int num3 = index + count - num;
		if (num3 < 0)
		{
			_firstIndex -= count;
			_startUUnstable = true;
			int num4 = _firstIndex;
			for (int i = 0; i < _elements.Count; i++)
			{
				Control control = _elements[i];
				if (control != null)
				{
					updateElementIndex(control, num4 - count, num4);
				}
				num4++;
			}
		}
		else
		{
			if (num2 >= _elements.Count)
			{
				return;
			}
			int num5 = Math.Max(num2, 0);
			int num6 = Math.Min(num3, _elements.Count);
			for (int j = num5; j < num6; j++)
			{
				Control control2 = _elements[j];
				if (control2 != null)
				{
					_elements[j] = null;
					recycleElement(control2);
				}
			}
			_elements.RemoveRange(num5, num6 - num5);
			_sizes.RemoveRange(num5, num6 - num5);
			if (num2 <= 0 && num6 < lastIndex)
			{
				num = (_firstIndex = index);
				_startUUnstable = true;
			}
			num6 = _elements.Count;
			int num7 = num + num5;
			for (int k = num5; k < num6; k++)
			{
				Control control3 = _elements[k];
				if (control3 != null)
				{
					updateElementIndex(control3, num7 + count, num7);
				}
				num7++;
			}
		}
	}

	public void ItemsReset(Action<Control> recycleElement)
	{
		if (_elements == null || _elements.Count == 0)
		{
			return;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			Control control = _elements[i];
			if (control != null)
			{
				_elements[i] = null;
				recycleElement(control);
			}
		}
		_startU = (_firstIndex = 0);
		_elements?.Clear();
		_sizes?.Clear();
	}

	public void RecycleElementsBefore(int index, Action<Control, int> recycleElement)
	{
		if (index <= FirstIndex || _elements == null || _elements.Count == 0)
		{
			return;
		}
		if (index > LastIndex)
		{
			RecycleAllElements(recycleElement);
			return;
		}
		int num = index - FirstIndex;
		for (int i = 0; i < num; i++)
		{
			Control control = _elements[i];
			if (control != null)
			{
				_elements[i] = null;
				recycleElement(control, i + FirstIndex);
			}
		}
		_elements.RemoveRange(0, num);
		_sizes.RemoveRange(0, num);
		_firstIndex = index;
	}

	public void RecycleElementsAfter(int index, Action<Control, int> recycleElement)
	{
		if (index >= LastIndex || _elements == null || _elements.Count == 0)
		{
			return;
		}
		if (index < FirstIndex)
		{
			RecycleAllElements(recycleElement);
			return;
		}
		int num = index + 1 - FirstIndex;
		int count = _elements.Count;
		for (int i = num; i < count; i++)
		{
			Control control = _elements[i];
			if (control != null)
			{
				_elements[i] = null;
				recycleElement(control, i + FirstIndex);
			}
		}
		_elements.RemoveRange(num, _elements.Count - num);
		_sizes.RemoveRange(num, _sizes.Count - num);
	}

	public void RecycleAllElements(Action<Control, int> recycleElement)
	{
		if (_elements == null || _elements.Count == 0)
		{
			return;
		}
		for (int i = 0; i < _elements.Count; i++)
		{
			Control control = _elements[i];
			if (control != null)
			{
				_elements[i] = null;
				recycleElement(control, i + FirstIndex);
			}
		}
		_startU = (_firstIndex = 0);
		_elements?.Clear();
		_sizes?.Clear();
	}

	public void ResetForReuse()
	{
		_startU = (_firstIndex = 0);
		_startUUnstable = false;
		_elements?.Clear();
		_sizes?.Clear();
	}
}
