namespace Avalonia.Automation.Provider;

public interface IValueProvider
{
	bool IsReadOnly { get; }

	string? Value { get; }

	void SetValue(string? value);
}
