using System;
using System.IO;
using System.Text;

namespace Avalonia.Markup.Xaml;

public class RuntimeXamlLoaderDocument
{
	public Uri? BaseUri { get; set; }

	public object? RootInstance { get; set; }

	public Stream XamlStream { get; }

	public IServiceProvider? ServiceProvider { get; set; }

	public RuntimeXamlLoaderDocument(string xaml)
	{
		XamlStream = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
	}

	public RuntimeXamlLoaderDocument(Uri? baseUri, string xaml)
		: this(xaml)
	{
		BaseUri = baseUri;
	}

	public RuntimeXamlLoaderDocument(object? rootInstance, string xaml)
		: this(xaml)
	{
		RootInstance = rootInstance;
	}

	public RuntimeXamlLoaderDocument(Uri? baseUri, object? rootInstance, string xaml)
		: this(baseUri, xaml)
	{
		RootInstance = rootInstance;
	}

	public RuntimeXamlLoaderDocument(Stream stream)
	{
		XamlStream = stream;
	}

	public RuntimeXamlLoaderDocument(Uri? baseUri, Stream stream)
		: this(stream)
	{
		BaseUri = baseUri;
	}

	public RuntimeXamlLoaderDocument(object? rootInstance, Stream stream)
		: this(stream)
	{
		RootInstance = rootInstance;
	}

	public RuntimeXamlLoaderDocument(Uri? baseUri, object? rootInstance, Stream stream)
		: this(baseUri, stream)
	{
		RootInstance = rootInstance;
	}
}
