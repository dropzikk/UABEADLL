using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable("IAssetLoader interface and AvaloniaLocator usage is considered unstable. Please use AssetLoader static class instead.")]
public interface IAssetLoader
{
	void SetDefaultAssembly(Assembly assembly);

	bool Exists(Uri uri, Uri? baseUri = null);

	Stream Open(Uri uri, Uri? baseUri = null);

	(Stream stream, Assembly assembly) OpenAndGetAssembly(Uri uri, Uri? baseUri = null);

	Assembly? GetAssembly(Uri uri, Uri? baseUri = null);

	IEnumerable<Uri> GetAssets(Uri uri, Uri? baseUri);
}
