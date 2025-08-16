using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Avalonia.Themes.Fluent;

internal class ColorPaletteResourcesCollection : AvaloniaDictionary<ThemeVariant, ColorPaletteResources>, IResourceProvider, IResourceNode
{
	public bool HasResources => base.Count > 0;

	public IResourceHost? Owner { get; private set; }

	public event EventHandler? OwnerChanged;

	public ColorPaletteResourcesCollection()
		: base(2)
	{
		this.ForEachItem(delegate(ThemeVariant key, ColorPaletteResources x)
		{
			if (Owner != null)
			{
				x.PropertyChanged += Palette_PropertyChanged;
			}
			if (key != ThemeVariant.Dark && key != ThemeVariant.Light)
			{
				throw new InvalidOperationException("FluentTheme.Palettes only supports Light and Dark variants.");
			}
		}, delegate(ThemeVariant _, ColorPaletteResources x)
		{
			if (Owner != null)
			{
				x.PropertyChanged -= Palette_PropertyChanged;
			}
		}, delegate
		{
			throw new NotSupportedException("Dictionary reset not supported");
		});
	}

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		if (theme == null || theme == ThemeVariant.Default)
		{
			theme = ThemeVariant.Light;
		}
		if (TryGetValue(theme, out ColorPaletteResources value2) && value2.TryGetResource(key, theme, out value))
		{
			return true;
		}
		value = null;
		return false;
	}

	public void AddOwner(IResourceHost owner)
	{
		Owner = owner;
		this.OwnerChanged?.Invoke(this, EventArgs.Empty);
	}

	public void RemoveOwner(IResourceHost owner)
	{
		Owner = null;
		this.OwnerChanged?.Invoke(this, EventArgs.Empty);
	}

	private void Palette_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ColorPaletteResources.AccentProperty)
		{
			Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
		}
	}
}
