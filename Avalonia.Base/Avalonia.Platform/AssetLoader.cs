using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Avalonia.Platform;

public static class AssetLoader
{
	private static IAssetLoader GetAssetLoader()
	{
		return AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();
	}

	public static void SetDefaultAssembly(Assembly assembly)
	{
		GetAssetLoader().SetDefaultAssembly(assembly);
	}

	public static bool Exists(Uri uri, Uri? baseUri = null)
	{
		return GetAssetLoader().Exists(uri, baseUri);
	}

	public static Stream Open(Uri uri, Uri? baseUri = null)
	{
		return GetAssetLoader().Open(uri, baseUri);
	}

	public static (Stream stream, Assembly assembly) OpenAndGetAssembly(Uri uri, Uri? baseUri = null)
	{
		return GetAssetLoader().OpenAndGetAssembly(uri, baseUri);
	}

	public static Assembly? GetAssembly(Uri uri, Uri? baseUri = null)
	{
		return GetAssetLoader().GetAssembly(uri, baseUri);
	}

	public static IEnumerable<Uri> GetAssets(Uri uri, Uri? baseUri)
	{
		return GetAssetLoader().GetAssets(uri, baseUri);
	}

	internal static void RegisterResUriParsers()
	{
		if (!UriParser.IsKnownScheme("avares"))
		{
			UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority | GenericUriParserOptions.NoUserInfo | GenericUriParserOptions.NoPort | GenericUriParserOptions.NoQuery | GenericUriParserOptions.NoFragment), "avares", -1);
		}
	}
}
