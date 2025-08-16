using Avalonia.Data.Core.Plugins;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class TaskStreamPathElement<T> : IStronglyTypedStreamElement, ICompiledBindingPathElement
{
	public static readonly TaskStreamPathElement<T> Instance = new TaskStreamPathElement<T>();

	public IStreamPlugin CreatePlugin()
	{
		return new TaskStreamPlugin<T>();
	}
}
