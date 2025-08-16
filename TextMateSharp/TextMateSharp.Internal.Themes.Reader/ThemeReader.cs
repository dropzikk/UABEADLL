using System.IO;
using TextMateSharp.Internal.Parser.Json;
using TextMateSharp.Themes;

namespace TextMateSharp.Internal.Themes.Reader;

public class ThemeReader
{
	public static IRawTheme ReadThemeSync(StreamReader reader)
	{
		return new JSONPListParser<IRawTheme>(theme: true).Parse(reader);
	}
}
