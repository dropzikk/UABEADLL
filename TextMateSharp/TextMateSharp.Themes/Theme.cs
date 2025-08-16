using System.Collections.Generic;
using TextMateSharp.Registry;

namespace TextMateSharp.Themes;

public class Theme
{
	private ParsedTheme _theme;

	private ParsedTheme _include;

	private ColorMap _colorMap;

	public static Theme CreateFromRawTheme(IRawTheme source, IRegistryOptions registryOptions)
	{
		ColorMap colorMap = new ColorMap();
		ParsedTheme theme = ParsedTheme.CreateFromParsedTheme(ParsedTheme.ParseTheme(source, 0), colorMap);
		ParsedTheme include = ParsedTheme.CreateFromParsedTheme(ParsedTheme.ParseInclude(source, registryOptions, 0), colorMap);
		return new Theme(colorMap, theme, include);
	}

	private Theme(ColorMap colorMap, ParsedTheme theme, ParsedTheme include)
	{
		_colorMap = colorMap;
		_theme = theme;
		_include = include;
	}

	public List<ThemeTrieElementRule> Match(IList<string> scopeNames)
	{
		List<ThemeTrieElementRule> result = new List<ThemeTrieElementRule>();
		for (int i = scopeNames.Count - 1; i >= 0; i--)
		{
			result.AddRange(_theme.Match(scopeNames[i]));
		}
		for (int i2 = scopeNames.Count - 1; i2 >= 0; i2--)
		{
			result.AddRange(_include.Match(scopeNames[i2]));
		}
		return result;
	}

	public ICollection<string> GetColorMap()
	{
		return _colorMap.GetColorMap();
	}

	public int GetColorId(string color)
	{
		return _colorMap.GetId(color);
	}

	public string GetColor(int id)
	{
		return _colorMap.GetColor(id);
	}

	internal ThemeTrieElementRule GetDefaults()
	{
		return _theme.GetDefaults();
	}
}
