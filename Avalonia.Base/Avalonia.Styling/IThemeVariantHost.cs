using System;
using Avalonia.Controls;

namespace Avalonia.Styling;

public interface IThemeVariantHost : IResourceHost, IResourceNode
{
	ThemeVariant ActualThemeVariant { get; }

	event EventHandler? ActualThemeVariantChanged;
}
