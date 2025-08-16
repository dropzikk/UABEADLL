using Avalonia.Input.Platform;

namespace Avalonia.Diagnostics.ViewModels;

internal class SetterViewModel : ViewModelBase
{
	private bool _isActive;

	private bool _isVisible;

	private IClipboard? _clipboard;

	public AvaloniaProperty Property { get; }

	public string Name { get; }

	public object? Value { get; }

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isActive, value, "IsActive");
		}
	}

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isVisible, value, "IsVisible");
		}
	}

	public SetterViewModel(AvaloniaProperty property, object? value, IClipboard? clipboard)
	{
		Property = property;
		Name = property.Name;
		Value = value;
		IsActive = true;
		IsVisible = true;
		_clipboard = clipboard;
	}

	public virtual void CopyValue()
	{
		string text = Value?.ToString();
		if (text != null)
		{
			CopyToClipboard(text);
		}
	}

	public void CopyPropertyName()
	{
		CopyToClipboard(Property.Name);
	}

	protected void CopyToClipboard(string value)
	{
		_clipboard?.SetTextAsync(value);
	}
}
