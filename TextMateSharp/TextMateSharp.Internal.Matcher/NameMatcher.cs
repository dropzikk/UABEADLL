using System.Collections.Generic;
using System.Linq;
using TextMateSharp.Internal.Utils;

namespace TextMateSharp.Internal.Matcher;

public class NameMatcher : IMatchesName<List<string>>
{
	public static IMatchesName<List<string>> Default = new NameMatcher();

	public bool Match(ICollection<string> identifers, List<string> scopes)
	{
		if (scopes.Count < identifers.Count)
		{
			return false;
		}
		int lastIndex = 0;
		return identifers.All(delegate(string identifier)
		{
			for (int i = lastIndex; i < scopes.Count; i++)
			{
				if (ScopesAreMatching(scopes[i], identifier))
				{
					lastIndex++;
					return true;
				}
			}
			return false;
		});
	}

	private bool ScopesAreMatching(string thisScopeName, string scopeName)
	{
		if (thisScopeName == null)
		{
			return false;
		}
		if (thisScopeName.Equals(scopeName))
		{
			return true;
		}
		int len = scopeName.Length;
		if (thisScopeName.Length > len && thisScopeName.SubstringAtIndexes(0, len).Equals(scopeName))
		{
			return thisScopeName[len] == '.';
		}
		return false;
	}
}
