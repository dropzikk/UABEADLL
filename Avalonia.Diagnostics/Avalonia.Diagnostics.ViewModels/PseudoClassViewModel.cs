using Avalonia.Controls;

namespace Avalonia.Diagnostics.ViewModels;

internal class PseudoClassViewModel : ViewModelBase
{
	private readonly IPseudoClasses _pseudoClasses;

	private readonly StyledElement _source;

	private bool _isActive;

	private bool _isUpdating;

	public string Name { get; }

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			RaiseAndSetIfChanged(ref _isActive, value, "IsActive");
			if (!_isUpdating)
			{
				_pseudoClasses.Set(Name, value);
			}
		}
	}

	public PseudoClassViewModel(string name, StyledElement source)
	{
		Name = name;
		_source = source;
		_pseudoClasses = _source.Classes;
		Update();
	}

	public void Update()
	{
		try
		{
			_isUpdating = true;
			IsActive = _source.Classes.Contains(Name);
		}
		finally
		{
			_isUpdating = false;
		}
	}
}
