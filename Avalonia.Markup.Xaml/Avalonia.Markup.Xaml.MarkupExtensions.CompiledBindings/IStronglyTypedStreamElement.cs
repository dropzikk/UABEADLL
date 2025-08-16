using Avalonia.Data.Core.Plugins;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal interface IStronglyTypedStreamElement : ICompiledBindingPathElement
{
	IStreamPlugin CreatePlugin();
}
