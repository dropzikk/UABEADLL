using Avalonia.Data.Core.Plugins;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class ObservableStreamPathElement<T> : IStronglyTypedStreamElement, ICompiledBindingPathElement
{
	public static readonly ObservableStreamPathElement<T> Instance = new ObservableStreamPathElement<T>();

	public IStreamPlugin CreatePlugin()
	{
		return new ObservableStreamPlugin<T>();
	}
}
