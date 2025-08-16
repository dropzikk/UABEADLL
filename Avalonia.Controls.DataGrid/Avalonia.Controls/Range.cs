namespace Avalonia.Controls;

internal class Range<T>
{
	public int Count => UpperBound - LowerBound + 1;

	public int LowerBound { get; set; }

	public int UpperBound { get; set; }

	public T Value { get; set; }

	public Range(int lowerBound, int upperBound, T value)
	{
		LowerBound = lowerBound;
		UpperBound = upperBound;
		Value = value;
	}

	public bool ContainsIndex(int index)
	{
		if (LowerBound <= index)
		{
			return UpperBound >= index;
		}
		return false;
	}

	public bool ContainsValue(object value)
	{
		if (Value == null)
		{
			return value == null;
		}
		return Value.Equals(value);
	}

	public Range<T> Copy()
	{
		return new Range<T>(LowerBound, UpperBound, Value);
	}
}
