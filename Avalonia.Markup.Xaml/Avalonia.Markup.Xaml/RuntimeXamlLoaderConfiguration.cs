using System.Reflection;

namespace Avalonia.Markup.Xaml;

public class RuntimeXamlLoaderConfiguration
{
	public Assembly? LocalAssembly { get; set; }

	public bool UseCompiledBindingsByDefault { get; set; }

	public bool DesignMode { get; set; }
}
