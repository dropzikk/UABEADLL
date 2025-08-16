using Avalonia.Metadata;
using Avalonia.Styling;

namespace Avalonia.Controls;

[Unstable("This XAML-only API might be removed in the future minor updates.")]
public interface IThemeVariantProvider : IResourceProvider, IResourceNode
{
	ThemeVariant? Key { get; set; }
}
