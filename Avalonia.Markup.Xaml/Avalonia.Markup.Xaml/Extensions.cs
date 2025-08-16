using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Styling;

namespace Avalonia.Markup.Xaml;

internal static class Extensions
{
	public static T? GetService<T>(this IServiceProvider sp)
	{
		return (T)sp.GetService(typeof(T));
	}

	public static T GetRequiredService<T>(this IServiceProvider sp)
	{
		T service = sp.GetService<T>();
		if (service == null)
		{
			throw new InvalidOperationException($"Service {typeof(T)} hasn't been registered");
		}
		return service;
	}

	public static Uri? GetContextBaseUri(this IServiceProvider ctx)
	{
		return ctx.GetService<IUriContext>()?.BaseUri;
	}

	public static T? GetFirstParent<T>(this IServiceProvider ctx) where T : class
	{
		IAvaloniaXamlIlParentStackProvider? service = ctx.GetService<IAvaloniaXamlIlParentStackProvider>();
		if (service == null)
		{
			return null;
		}
		return service.Parents.OfType<T>().FirstOrDefault();
	}

	public static T? GetLastParent<T>(this IServiceProvider ctx) where T : class
	{
		IAvaloniaXamlIlParentStackProvider? service = ctx.GetService<IAvaloniaXamlIlParentStackProvider>();
		if (service == null)
		{
			return null;
		}
		return service.Parents.OfType<T>().LastOrDefault();
	}

	public static IEnumerable<T> GetParents<T>(this IServiceProvider sp)
	{
		return sp.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>() ?? Enumerable.Empty<T>();
	}

	public static bool IsInControlTemplate(this IServiceProvider sp)
	{
		return sp.GetService<IAvaloniaXamlIlControlTemplateProvider>() != null;
	}

	[RequiresUnreferencedCode("XamlTypeResolver might require unreferenced code.")]
	public static Type ResolveType(this IServiceProvider ctx, string? namespacePrefix, string type)
	{
		IXamlTypeResolver requiredService = ctx.GetRequiredService<IXamlTypeResolver>();
		string qualifiedTypeName = (string.IsNullOrEmpty(namespacePrefix) ? type : (namespacePrefix + ":" + type));
		return requiredService.Resolve(qualifiedTypeName);
	}

	public static object? GetDefaultAnchor(this IServiceProvider provider)
	{
		object firstParent = provider.GetFirstParent<Control>();
		if (firstParent == null)
		{
			firstParent = provider.GetFirstParent<IDataContextProvider>();
		}
		return firstParent ?? (provider.GetService<IRootObjectProvider>()?.RootObject as IStyle) ?? provider.GetLastParent<IStyle>();
	}
}
