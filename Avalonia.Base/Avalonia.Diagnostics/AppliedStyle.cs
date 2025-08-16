using Avalonia.Styling;

namespace Avalonia.Diagnostics;

public class AppliedStyle
{
	private readonly IStyleInstance _instance;

	public bool HasActivator => _instance.HasActivator;

	public bool IsActive => _instance.IsActive;

	public StyleBase Style => (StyleBase)_instance.Source;

	internal AppliedStyle(IStyleInstance instance)
	{
		_instance = instance;
	}
}
