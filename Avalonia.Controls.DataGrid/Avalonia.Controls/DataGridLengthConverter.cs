using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class DataGridLengthConverter : TypeConverter
{
	private static string _starSuffix = "*";

	private static string[] _valueInvariantUnitStrings = new string[3] { "auto", "sizetocells", "sizetoheader" };

	private static DataGridLength[] _valueInvariantDataGridLengths = new DataGridLength[3]
	{
		DataGridLength.Auto,
		DataGridLength.SizeToCells,
		DataGridLength.SizeToHeader
	};

	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		TypeCode typeCode = Type.GetTypeCode(sourceType);
		if ((uint)(typeCode - 7) <= 8u || typeCode == TypeCode.String)
		{
			return true;
		}
		return false;
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(string);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value == null)
		{
			throw DataGridError.DataGridLengthConverter.CannotConvertFrom("(null)");
		}
		if (value is string text)
		{
			string text2 = text.Trim();
			if (text2.EndsWith(_starSuffix, StringComparison.Ordinal))
			{
				string value2 = text2.Substring(0, text2.Length - _starSuffix.Length);
				double value3 = ((!string.IsNullOrEmpty(value2)) ? Convert.ToDouble(value2, culture ?? CultureInfo.CurrentCulture) : 1.0);
				return new DataGridLength(value3, DataGridLengthUnitType.Star);
			}
			for (int i = 0; i < _valueInvariantUnitStrings.Length; i++)
			{
				if (text2.Equals(_valueInvariantUnitStrings[i], StringComparison.OrdinalIgnoreCase))
				{
					return _valueInvariantDataGridLengths[i];
				}
			}
		}
		double num = Convert.ToDouble(value, culture ?? CultureInfo.CurrentCulture);
		if (double.IsNaN(num))
		{
			return DataGridLength.Auto;
		}
		return new DataGridLength(num);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == null)
		{
			throw new ArgumentNullException("destinationType");
		}
		if (destinationType != typeof(string))
		{
			throw DataGridError.DataGridLengthConverter.CannotConvertTo(destinationType.ToString());
		}
		DataGridLength? dataGridLength = value as DataGridLength?;
		if (!dataGridLength.HasValue)
		{
			throw DataGridError.DataGridLengthConverter.InvalidDataGridLength("value");
		}
		switch (dataGridLength.Value.UnitType)
		{
		case DataGridLengthUnitType.Auto:
			return "Auto";
		case DataGridLengthUnitType.SizeToHeader:
			return "SizeToHeader";
		case DataGridLengthUnitType.SizeToCells:
			return "SizeToCells";
		case DataGridLengthUnitType.Star:
			if (!MathUtilities.AreClose(1.0, dataGridLength.Value.Value))
			{
				return Convert.ToString(dataGridLength.Value.Value, culture ?? CultureInfo.CurrentCulture) + _starSuffix;
			}
			return _starSuffix;
		default:
			return Convert.ToString(dataGridLength.Value.Value, culture ?? CultureInfo.CurrentCulture);
		}
	}
}
