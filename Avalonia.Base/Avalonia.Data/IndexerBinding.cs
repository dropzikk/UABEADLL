using Avalonia.Reactive;

namespace Avalonia.Data;

internal class IndexerBinding : IBinding
{
	private AvaloniaObject Source { get; }

	public AvaloniaProperty Property { get; }

	private BindingMode Mode { get; }

	public IndexerBinding(AvaloniaObject source, AvaloniaProperty property, BindingMode mode)
	{
		Source = source;
		Property = property;
		Mode = mode;
	}

	public InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false)
	{
		return new InstancedBinding(new CombinedSubject<object>(new AnonymousObserver<object>(delegate(object? x)
		{
			Source.SetValue(Property, x);
		}), Source.GetObservable(Property)), Mode, BindingPriority.LocalValue);
	}
}
