using System;
using System.Globalization;
using Avalonia.Controls.Utils;
using Avalonia.Data.Converters;

namespace Avalonia.Collections;

public class DataGridPathGroupDescription : DataGridGroupDescription
{
	private string _propertyPath;

	private Type _propertyType;

	private IValueConverter _valueConverter;

	private StringComparison _stringComparison = StringComparison.Ordinal;

	public override string PropertyName => _propertyPath;

	public IValueConverter ValueConverter
	{
		get
		{
			return _valueConverter;
		}
		set
		{
			_valueConverter = value;
		}
	}

	public DataGridPathGroupDescription(string propertyPath)
	{
		_propertyPath = propertyPath;
	}

	public override object GroupKeyFromItem(object item, int level, CultureInfo culture)
	{
		object obj = GetKey(item);
		if (obj == null)
		{
			obj = item;
		}
		IValueConverter valueConverter = ValueConverter;
		if (valueConverter != null)
		{
			obj = valueConverter.Convert(obj, typeof(object), level, culture);
		}
		return obj;
		object GetKey(object o)
		{
			if (o == null)
			{
				return null;
			}
			if (_propertyType == null)
			{
				_propertyType = GetPropertyType(o);
			}
			return InvokePath(o, _propertyPath, _propertyType);
		}
	}

	public override bool KeysMatch(object groupKey, object itemKey)
	{
		if (groupKey is string a && itemKey is string b)
		{
			return string.Equals(a, b, _stringComparison);
		}
		return base.KeysMatch(groupKey, itemKey);
	}

	private Type GetPropertyType(object o)
	{
		return o.GetType().GetNestedPropertyType(_propertyPath);
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
}
