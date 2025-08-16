using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls.Utils;

namespace Avalonia.Collections;

public abstract class DataGridSortDescription
{
	private class CultureSensitiveComparer : Comparer<object>
	{
		private CultureInfo _culture;

		public CultureSensitiveComparer(CultureInfo culture)
		{
			_culture = culture ?? CultureInfo.InvariantCulture;
		}

		public override int Compare(object x, object y)
		{
			if (x == null)
			{
				if (y != null)
				{
					return -1;
				}
				return 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (x.GetType() == typeof(string) && y.GetType() == typeof(string))
			{
				return _culture.CompareInfo.Compare((string)x, (string)y);
			}
			return Comparer<object>.Default.Compare(x, y);
		}
	}

	private class DataGridPathSortDescription : DataGridSortDescription
	{
		private readonly ListSortDirection _direction;

		private readonly string _propertyPath;

		private readonly Lazy<CultureSensitiveComparer> _cultureSensitiveComparer;

		private readonly Lazy<IComparer<object>> _comparer;

		private Type _propertyType;

		private IComparer _internalComparer;

		private IComparer<object> _internalComparerTyped;

		private IComparer<object> InternalComparer
		{
			get
			{
				if (_internalComparerTyped == null && _internalComparer != null)
				{
					if (_internalComparer is IComparer<object> internalComparerTyped)
					{
						_internalComparerTyped = internalComparerTyped;
					}
					else
					{
						_internalComparerTyped = Comparer<object>.Create((object x, object y) => _internalComparer.Compare(x, y));
					}
				}
				return _internalComparerTyped;
			}
		}

		public override string PropertyPath => _propertyPath;

		public override IComparer<object> Comparer => _comparer.Value;

		public override ListSortDirection Direction => _direction;

		public DataGridPathSortDescription(string propertyPath, ListSortDirection direction, IComparer internalComparer, CultureInfo culture)
		{
			DataGridPathSortDescription dataGridPathSortDescription = this;
			_propertyPath = propertyPath;
			_direction = direction;
			_cultureSensitiveComparer = new Lazy<CultureSensitiveComparer>(() => new CultureSensitiveComparer(culture ?? CultureInfo.CurrentCulture));
			_internalComparer = internalComparer;
			_comparer = new Lazy<IComparer<object>>(() => Comparer<object>.Create((object x, object y) => dataGridPathSortDescription.Compare(x, y)));
		}

		private DataGridPathSortDescription(DataGridPathSortDescription inner, ListSortDirection direction)
		{
			_propertyPath = inner._propertyPath;
			_direction = direction;
			_propertyType = inner._propertyType;
			_cultureSensitiveComparer = inner._cultureSensitiveComparer;
			_internalComparer = inner._internalComparer;
			_internalComparerTyped = inner._internalComparerTyped;
			_comparer = new Lazy<IComparer<object>>(() => Comparer<object>.Create((object x, object y) => Compare(x, y)));
		}

		private object GetValue(object o)
		{
			if (o == null)
			{
				return null;
			}
			if (base.HasPropertyPath)
			{
				return InvokePath(o, _propertyPath, _propertyType);
			}
			if (_propertyType == o.GetType())
			{
				return o;
			}
			return null;
		}

		private IComparer GetComparerForType(Type type)
		{
			if (type == typeof(string))
			{
				return _cultureSensitiveComparer.Value;
			}
			return typeof(Comparer<>).MakeGenericType(type).GetProperty("Default").GetValue(null, null) as IComparer;
		}

		private Type GetPropertyType(object o)
		{
			return o.GetType().GetNestedPropertyType(_propertyPath);
		}

		private int Compare(object x, object y)
		{
			int num = 0;
			if (_propertyType == null)
			{
				if (x != null)
				{
					_propertyType = GetPropertyType(x);
				}
				if (_propertyType == null && y != null)
				{
					_propertyType = GetPropertyType(y);
				}
			}
			object value = GetValue(x);
			object value2 = GetValue(y);
			if (_propertyType != null && _internalComparer == null)
			{
				_internalComparer = GetComparerForType(_propertyType);
			}
			num = _internalComparer?.Compare(value, value2) ?? 0;
			if (Direction == ListSortDirection.Descending)
			{
				return -num;
			}
			return num;
		}

		internal override void Initialize(Type itemType)
		{
			base.Initialize(itemType);
			if (_propertyType == null)
			{
				_propertyType = itemType.GetNestedPropertyType(_propertyPath);
			}
			if (_internalComparer == null && _propertyType != null)
			{
				_internalComparer = GetComparerForType(_propertyType);
			}
		}

		public override IOrderedEnumerable<object> OrderBy(IEnumerable<object> seq)
		{
			if (Direction == ListSortDirection.Descending)
			{
				return seq.OrderByDescending((object o) => GetValue(o), InternalComparer);
			}
			return seq.OrderBy((object o) => GetValue(o), InternalComparer);
		}

		public override IOrderedEnumerable<object> ThenBy(IOrderedEnumerable<object> seq)
		{
			if (Direction == ListSortDirection.Descending)
			{
				return seq.ThenByDescending((object o) => GetValue(o), InternalComparer);
			}
			return seq.ThenBy((object o) => GetValue(o), InternalComparer);
		}

		public override DataGridSortDescription SwitchSortDirection()
		{
			ListSortDirection direction = ((_direction == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending);
			return new DataGridPathSortDescription(this, direction);
		}
	}

	public virtual string PropertyPath => null;

	public virtual ListSortDirection Direction => ListSortDirection.Ascending;

	public bool HasPropertyPath => !string.IsNullOrEmpty(PropertyPath);

	public abstract IComparer<object> Comparer { get; }

	public virtual IOrderedEnumerable<object> OrderBy(IEnumerable<object> seq)
	{
		return seq.OrderBy((object o) => o, Comparer);
	}

	public virtual IOrderedEnumerable<object> ThenBy(IOrderedEnumerable<object> seq)
	{
		return seq.ThenBy((object o) => o, Comparer);
	}

	public virtual DataGridSortDescription SwitchSortDirection()
	{
		return this;
	}

	internal virtual void Initialize(Type itemType)
	{
	}

	private static object InvokePath(object item, string propertyPath, Type propertyType)
	{
		Exception exception;
		object nestedPropertyValue = TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out exception);
		if (exception != null)
		{
			throw exception;
		}
		return nestedPropertyValue;
	}

	public static DataGridSortDescription FromPath(string propertyPath, ListSortDirection direction = ListSortDirection.Ascending, CultureInfo culture = null)
	{
		return new DataGridPathSortDescription(propertyPath, direction, null, culture);
	}

	public static DataGridSortDescription FromPath(string propertyPath, ListSortDirection direction, IComparer comparer)
	{
		return new DataGridPathSortDescription(propertyPath, direction, comparer, null);
	}

	public static DataGridSortDescription FromComparer(IComparer comparer, ListSortDirection direction = ListSortDirection.Ascending)
	{
		return new DataGridComparerSortDescription(comparer, direction);
	}
}
