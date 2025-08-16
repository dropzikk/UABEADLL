using Avalonia.Data;

namespace Avalonia;

public abstract class AvaloniaPropertyMetadata
{
	private BindingMode _defaultBindingMode;

	public BindingMode DefaultBindingMode
	{
		get
		{
			if (_defaultBindingMode != 0)
			{
				return _defaultBindingMode;
			}
			return BindingMode.OneWay;
		}
	}

	public bool? EnableDataValidation { get; private set; }

	public AvaloniaPropertyMetadata(BindingMode defaultBindingMode = BindingMode.Default, bool? enableDataValidation = null)
	{
		_defaultBindingMode = defaultBindingMode;
		EnableDataValidation = enableDataValidation;
	}

	public virtual void Merge(AvaloniaPropertyMetadata baseMetadata, AvaloniaProperty property)
	{
		if (_defaultBindingMode == BindingMode.Default)
		{
			_defaultBindingMode = baseMetadata.DefaultBindingMode;
		}
		if (!EnableDataValidation.HasValue)
		{
			bool? flag = (EnableDataValidation = baseMetadata.EnableDataValidation);
		}
	}

	public abstract AvaloniaPropertyMetadata GenerateTypeSafeMetadata();
}
