using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Avalonia.Markup.Xaml.Styling;

[RequiresUnreferencedCode("StyleInclude and ResourceInclude use AvaloniaXamlLoader.Load which dynamically loads referenced assembly with Avalonia resources. Note, StyleInclude and ResourceInclude defined in XAML are resolved compile time and are safe with trimming and AOT.")]
public class StyleInclude : IStyle, IResourceNode, IResourceProvider
{
	private readonly IServiceProvider? _serviceProvider;

	private readonly Uri? _baseUri;

	private IStyle[]? _loaded;

	private bool _isLoading;

	public IResourceHost? Owner => (Loaded as IResourceProvider)?.Owner;

	public Uri? Source { get; set; }

	public IStyle Loaded
	{
		get
		{
			if (_loaded == null)
			{
				_isLoading = true;
				Uri uri = Source ?? throw new InvalidOperationException("StyleInclude.Source must be set.");
				IStyle style = (IStyle)AvaloniaXamlLoader.Load(_serviceProvider, uri, _baseUri);
				_loaded = new IStyle[1] { style };
				_isLoading = false;
			}
			return _loaded[0];
		}
	}

	bool IResourceNode.HasResources => Loaded.HasResources;

	IReadOnlyList<IStyle> IStyle.Children => _loaded ?? Array.Empty<IStyle>();

	public event EventHandler? OwnerChanged
	{
		add
		{
			if (Loaded is IResourceProvider resourceProvider)
			{
				resourceProvider.OwnerChanged += value;
			}
		}
		remove
		{
			if (Loaded is IResourceProvider resourceProvider)
			{
				resourceProvider.OwnerChanged -= value;
			}
		}
	}

	public StyleInclude(Uri? baseUri)
	{
		_baseUri = baseUri;
	}

	public StyleInclude(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_baseUri = serviceProvider.GetContextBaseUri();
	}

	public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
	{
		if (!_isLoading)
		{
			return Loaded.TryGetResource(key, theme, out value);
		}
		value = null;
		return false;
	}

	void IResourceProvider.AddOwner(IResourceHost owner)
	{
		(Loaded as IResourceProvider)?.AddOwner(owner);
	}

	void IResourceProvider.RemoveOwner(IResourceHost owner)
	{
		(Loaded as IResourceProvider)?.RemoveOwner(owner);
	}
}
