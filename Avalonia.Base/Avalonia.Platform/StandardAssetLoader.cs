using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Metadata;
using Avalonia.Platform.Internal;
using Avalonia.Utilities;

namespace Avalonia.Platform;

[Unstable("StandardAssetLoader is considered unstable. Please use AssetLoader static class instead.")]
public class StandardAssetLoader : IAssetLoader
{
	private readonly IAssemblyDescriptorResolver _assemblyDescriptorResolver;

	private AssemblyDescriptor? _defaultResmAssembly;

	internal StandardAssetLoader(IAssemblyDescriptorResolver resolver, Assembly? assembly = null)
	{
		if (assembly == null)
		{
			assembly = Assembly.GetEntryAssembly();
		}
		if (assembly != null)
		{
			_defaultResmAssembly = new AssemblyDescriptor(assembly);
		}
		_assemblyDescriptorResolver = resolver;
	}

	public StandardAssetLoader(Assembly? assembly = null)
		: this(new AssemblyDescriptorResolver(), assembly)
	{
	}

	public void SetDefaultAssembly(Assembly assembly)
	{
		_defaultResmAssembly = new AssemblyDescriptor(assembly);
	}

	public bool Exists(Uri uri, Uri? baseUri = null)
	{
		IAssetDescriptor assetDescriptor;
		return TryGetAsset(uri, baseUri, out assetDescriptor);
	}

	public Stream Open(Uri uri, Uri? baseUri = null)
	{
		return OpenAndGetAssembly(uri, baseUri).stream;
	}

	public (Stream stream, Assembly assembly) OpenAndGetAssembly(Uri uri, Uri? baseUri = null)
	{
		if (TryGetAsset(uri, baseUri, out IAssetDescriptor assetDescriptor))
		{
			return (stream: assetDescriptor.GetStream(), assembly: assetDescriptor.Assembly);
		}
		throw new FileNotFoundException($"The resource {uri} could not be found.");
	}

	public Assembly? GetAssembly(Uri uri, Uri? baseUri)
	{
		if (!uri.IsAbsoluteUri && baseUri != null)
		{
			uri = new Uri(baseUri, uri);
		}
		if (TryGetAssembly(uri, out IAssemblyDescriptor assembly))
		{
			return assembly.Assembly;
		}
		return null;
	}

	public IEnumerable<Uri> GetAssets(Uri uri, Uri? baseUri)
	{
		if (uri.IsAbsoluteResm())
		{
			if (!TryGetAssembly(uri, out IAssemblyDescriptor assembly))
			{
				assembly = _defaultResmAssembly;
			}
			return (from x in assembly?.Resources?.Where((KeyValuePair<string, IAssetDescriptor> x) => x.Key.Contains(uri.GetUnescapeAbsolutePath()))
				select new Uri("resm:" + x.Key + "?assembly=" + assembly.Name)) ?? Enumerable.Empty<Uri>();
		}
		uri = uri.EnsureAbsolute(baseUri);
		if (uri.IsAvares())
		{
			if (!TryGetResAsmAndPath(uri, out IAssemblyDescriptor assembly2, out string path))
			{
				return Enumerable.Empty<Uri>();
			}
			if (assembly2?.AvaloniaResources == null)
			{
				return Enumerable.Empty<Uri>();
			}
			if (path.Length > 0 && path[path.Length - 1] != '/')
			{
				path += "/";
			}
			return from r in assembly2.AvaloniaResources
				where r.Key.StartsWith(path, StringComparison.Ordinal)
				select r into x
				select new Uri("avares://" + assembly2.Name + x.Key);
		}
		return Enumerable.Empty<Uri>();
	}

	public static void RegisterResUriParsers()
	{
		AssetLoader.RegisterResUriParsers();
	}

	private bool TryGetAsset(Uri uri, Uri? baseUri, [NotNullWhen(true)] out IAssetDescriptor? assetDescriptor)
	{
		assetDescriptor = null;
		if (uri.IsAbsoluteResm())
		{
			if (!TryGetAssembly(uri, out IAssemblyDescriptor assembly) && !TryGetAssembly(baseUri, out assembly))
			{
				assembly = _defaultResmAssembly;
			}
			if (assembly?.Resources != null)
			{
				string absolutePath = uri.AbsolutePath;
				if (assembly.Resources.TryGetValue(absolutePath, out assetDescriptor))
				{
					return true;
				}
			}
		}
		uri = uri.EnsureAbsolute(baseUri);
		if (uri.IsAvares() && TryGetResAsmAndPath(uri, out IAssemblyDescriptor assembly2, out string path))
		{
			if (assembly2.AvaloniaResources == null)
			{
				return false;
			}
			if (assembly2.AvaloniaResources.TryGetValue(path, out assetDescriptor))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryGetResAsmAndPath(Uri uri, [NotNullWhen(true)] out IAssemblyDescriptor? assembly, out string path)
	{
		path = uri.GetUnescapeAbsolutePath();
		if (TryLoadAssembly(uri.Authority, out assembly))
		{
			return true;
		}
		return false;
	}

	private bool TryGetAssembly(Uri? uri, [NotNullWhen(true)] out IAssemblyDescriptor? assembly)
	{
		assembly = null;
		if (uri != null)
		{
			if (!uri.IsAbsoluteUri)
			{
				return false;
			}
			if (uri.IsAvares() && TryGetResAsmAndPath(uri, out assembly, out string _))
			{
				return true;
			}
			if (uri.IsResm())
			{
				string assemblyNameFromQuery = uri.GetAssemblyNameFromQuery();
				if (assemblyNameFromQuery.Length > 0 && TryLoadAssembly(assemblyNameFromQuery, out assembly))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool TryLoadAssembly(string assemblyName, [NotNullWhen(true)] out IAssemblyDescriptor? assembly)
	{
		assembly = null;
		try
		{
			assembly = _assemblyDescriptorResolver.GetAssembly(assemblyName);
			return true;
		}
		catch (Exception)
		{
		}
		return false;
	}
}
