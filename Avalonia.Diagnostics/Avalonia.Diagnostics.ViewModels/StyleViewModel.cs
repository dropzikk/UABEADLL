using System.Collections.Generic;

namespace Avalonia.Diagnostics.ViewModels;

internal class StyleViewModel : ViewModelBase
{
	private readonly AppliedStyle _styleInstance;

	private bool _isActive;

	private bool _isVisible;

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

	public string Name { get; }

	public List<SetterViewModel> Setters { get; }

	public StyleViewModel(AppliedStyle styleInstance, string name, List<SetterViewModel> setters)
	{
		_styleInstance = styleInstance;
		IsVisible = true;
		Name = name;
		Setters = setters;
		Update();
	}

	public void Update()
	{
		IsActive = _styleInstance.IsActive;
	}
}
