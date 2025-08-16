using Avalonia.Interactivity;

namespace Avalonia.Controls.Primitives;

public class TemplateAppliedEventArgs : RoutedEventArgs
{
	public INameScope NameScope { get; }

	public TemplateAppliedEventArgs(INameScope nameScope)
		: base(TemplatedControl.TemplateAppliedEvent)
	{
		NameScope = nameScope;
	}
}
