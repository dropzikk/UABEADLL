using System;
using System.ComponentModel;
using System.Globalization;

namespace Avalonia.Animation;

[TypeConverter(typeof(CueTypeConverter))]
public readonly record struct Cue : IEquatable<double>
{
	public double CueValue { get; }

	public Cue(double value)
	{
		if (value <= 1.0 && value >= 0.0)
		{
			CueValue = value;
			return;
		}
		throw new ArgumentException("This cue object's value should be within or equal to 0.0 and 1.0");
	}

	public static Cue Parse(string value, CultureInfo? culture)
	{
		string text = value;
		if (value.EndsWith("%"))
		{
			text = text.TrimEnd('%');
		}
		if (double.TryParse(text, NumberStyles.Float, culture, out var result))
		{
			return new Cue(result / 100.0);
		}
		throw new FormatException("Invalid Cue string \"" + value + "\"");
	}

	public bool Equals(double other)
	{
		return CueValue == other;
	}
}
