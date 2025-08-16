using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextMateSharp.Themes;

namespace TextMateSharp.Internal.Themes;

public class ThemeRaw : Dictionary<string, object>, IRawTheme, IRawThemeSetting, IThemeSetting
{
	private static string NAME = "name";

	private static string INCLUDE = "include";

	private static string SETTINGS = "settings";

	private static string TOKEN_COLORS = "tokenColors";

	private static string SCOPE = "scope";

	private static string FONT_STYLE = "fontStyle";

	private static string BACKGROUND = "background";

	private static string FOREGROUND = "foreground";

	public string GetName()
	{
		return TryGetObject<string>(NAME);
	}

	public string GetInclude()
	{
		return TryGetObject<string>(INCLUDE);
	}

	public ICollection<IRawThemeSetting> GetSettings()
	{
		return TryGetObject<ICollection>(SETTINGS)?.Cast<IRawThemeSetting>().ToList();
	}

	public ICollection<IRawThemeSetting> GetTokenColors()
	{
		return TryGetObject<ICollection>(TOKEN_COLORS)?.Cast<IRawThemeSetting>().ToList();
	}

	public object GetScope()
	{
		return TryGetObject<object>(SCOPE);
	}

	public IThemeSetting GetSetting()
	{
		return TryGetObject<IThemeSetting>(SETTINGS);
	}

	public object GetFontStyle()
	{
		return TryGetObject<object>(FONT_STYLE);
	}

	public string GetBackground()
	{
		return TryGetObject<string>(BACKGROUND);
	}

	public string GetForeground()
	{
		return TryGetObject<string>(FOREGROUND);
	}

	private T TryGetObject<T>(string key)
	{
		if (!TryGetValue(key, out var result))
		{
			return default(T);
		}
		return (T)result;
	}
}
