namespace Avalonia.Automation.Provider;

public interface IRangeValueProvider
{
	bool IsReadOnly { get; }

	double Minimum { get; }

	double Maximum { get; }

	double Value { get; }

	double LargeChange { get; }

	double SmallChange { get; }

	void SetValue(double value);
}
