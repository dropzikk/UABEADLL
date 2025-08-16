using System;
using System.ComponentModel;

namespace Avalonia.Controls;

[TypeConverter(typeof(DataGridLengthConverter))]
public struct DataGridLength : IEquatable<DataGridLength>
{
	private double _desiredValue;

	private double _displayValue;

	private double _unitValue;

	private DataGridLengthUnitType _unitType;

	private static readonly DataGridLength _auto = new DataGridLength(1.0, DataGridLengthUnitType.Auto);

	private static readonly DataGridLength _sizeToCells = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);

	private static readonly DataGridLength _sizeToHeader = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);

	internal const double DATAGRIDLENGTH_DefaultValue = 1.0;

	public static DataGridLength Auto => _auto;

	public double DesiredValue => _desiredValue;

	public double DisplayValue => _displayValue;

	public bool IsAbsolute => _unitType == DataGridLengthUnitType.Pixel;

	public bool IsAuto => _unitType == DataGridLengthUnitType.Auto;

	public bool IsSizeToCells => _unitType == DataGridLengthUnitType.SizeToCells;

	public bool IsSizeToHeader => _unitType == DataGridLengthUnitType.SizeToHeader;

	public bool IsStar => _unitType == DataGridLengthUnitType.Star;

	public static DataGridLength SizeToCells => _sizeToCells;

	public static DataGridLength SizeToHeader => _sizeToHeader;

	public DataGridLengthUnitType UnitType => _unitType;

	public double Value => _unitValue;

	public DataGridLength(double value)
		: this(value, DataGridLengthUnitType.Pixel)
	{
	}

	public DataGridLength(double value, DataGridLengthUnitType type)
		: this(value, type, (type == DataGridLengthUnitType.Pixel) ? value : double.NaN, (type == DataGridLengthUnitType.Pixel) ? value : double.NaN)
	{
	}

	public DataGridLength(double value, DataGridLengthUnitType type, double desiredValue, double displayValue)
	{
		if (double.IsNaN(value))
		{
			throw DataGridError.DataGrid.ValueCannotBeSetToNAN("value");
		}
		if (double.IsInfinity(value))
		{
			throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("value");
		}
		if (double.IsInfinity(desiredValue))
		{
			throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("desiredValue");
		}
		if (double.IsInfinity(displayValue))
		{
			throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("displayValue");
		}
		if (value < 0.0)
		{
			throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "value", 0);
		}
		if (desiredValue < 0.0)
		{
			throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("desiredValue", "desiredValue", 0);
		}
		if (displayValue < 0.0)
		{
			throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("displayValue", "displayValue", 0);
		}
		if (type != 0 && type != DataGridLengthUnitType.SizeToCells && type != DataGridLengthUnitType.SizeToHeader && type != DataGridLengthUnitType.Star && type != DataGridLengthUnitType.Pixel)
		{
			throw DataGridError.DataGridLength.InvalidUnitType("type");
		}
		_desiredValue = desiredValue;
		_displayValue = displayValue;
		_unitValue = ((type == DataGridLengthUnitType.Auto) ? 1.0 : value);
		_unitType = type;
	}

	public static bool operator ==(DataGridLength gl1, DataGridLength gl2)
	{
		if (gl1.UnitType == gl2.UnitType && gl1.Value == gl2.Value && gl1.DesiredValue == gl2.DesiredValue)
		{
			return gl1.DisplayValue == gl2.DisplayValue;
		}
		return false;
	}

	public static bool operator !=(DataGridLength gl1, DataGridLength gl2)
	{
		if (gl1.UnitType == gl2.UnitType && gl1.Value == gl2.Value && gl1.DesiredValue == gl2.DesiredValue)
		{
			return gl1.DisplayValue != gl2.DisplayValue;
		}
		return true;
	}

	public bool Equals(DataGridLength other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		DataGridLength? dataGridLength = obj as DataGridLength?;
		if (dataGridLength.HasValue)
		{
			DataGridLength value = this;
			DataGridLength? dataGridLength2 = dataGridLength;
			return value == dataGridLength2;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)((int)_unitValue + _unitType + (int)_desiredValue + (int)_displayValue);
	}
}
