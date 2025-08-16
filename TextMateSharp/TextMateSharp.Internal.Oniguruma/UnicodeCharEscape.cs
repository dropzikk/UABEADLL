using System.Text.RegularExpressions;

namespace TextMateSharp.Internal.Oniguruma;

public class UnicodeCharEscape
{
	private static Regex UNICODE_WITHOUT_BRACES_PATTERN = new Regex("\\\\x[A-Fa-f0-9]{2,8}");

	public static string AddBracesToUnicodePatterns(string pattern)
	{
		return UNICODE_WITHOUT_BRACES_PATTERN.Replace(pattern, delegate(Match m)
		{
			string text = "\\x";
			return text + "{" + m.Value.Substring(text.Length) + "}";
		});
	}

	internal static string ConstraintUnicodePatternLenght(string pattern)
	{
		return pattern.Replace("\\x{7fffffff}", "\\x{7ffff}");
	}
}
