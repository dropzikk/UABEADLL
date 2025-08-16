using System.Collections.Generic;
using TextMateSharp.Internal.Utils;

namespace TextMateSharp.Themes;

public class ThemeTrieElement
{
	private ThemeTrieElementRule mainRule;

	private List<ThemeTrieElementRule> rulesWithParentScopes;

	private Dictionary<string, ThemeTrieElement> children;

	public ThemeTrieElement(ThemeTrieElementRule mainRule)
		: this(mainRule, new List<ThemeTrieElementRule>(), new Dictionary<string, ThemeTrieElement>())
	{
	}

	public ThemeTrieElement(ThemeTrieElementRule mainRule, List<ThemeTrieElementRule> rulesWithParentScopes)
		: this(mainRule, rulesWithParentScopes, new Dictionary<string, ThemeTrieElement>())
	{
	}

	public ThemeTrieElement(ThemeTrieElementRule mainRule, List<ThemeTrieElementRule> rulesWithParentScopes, Dictionary<string, ThemeTrieElement> children)
	{
		this.mainRule = mainRule;
		this.rulesWithParentScopes = rulesWithParentScopes;
		this.children = children;
	}

	private static List<ThemeTrieElementRule> SortBySpecificity(List<ThemeTrieElementRule> arr)
	{
		if (arr.Count == 1)
		{
			return arr;
		}
		arr.Sort((ThemeTrieElementRule a, ThemeTrieElementRule b) => CmpBySpecificity(a, b));
		return arr;
	}

	private static int CmpBySpecificity(ThemeTrieElementRule a, ThemeTrieElementRule b)
	{
		if (a.scopeDepth == b.scopeDepth)
		{
			List<string> aParentScopes = a.parentScopes;
			List<string> bParentScopes = b.parentScopes;
			int aParentScopesLen = aParentScopes?.Count ?? 0;
			int bParentScopesLen = bParentScopes?.Count ?? 0;
			if (aParentScopesLen == bParentScopesLen)
			{
				for (int i = 0; i < aParentScopesLen; i++)
				{
					int aLen = aParentScopes[i].Length;
					int bLen = bParentScopes[i].Length;
					if (aLen != bLen)
					{
						return bLen - aLen;
					}
				}
			}
			return bParentScopesLen - aParentScopesLen;
		}
		return b.scopeDepth - a.scopeDepth;
	}

	public List<ThemeTrieElementRule> Match(string scope)
	{
		List<ThemeTrieElementRule> arr;
		if ("".Equals(scope))
		{
			arr = new List<ThemeTrieElementRule>();
			arr.Add(mainRule);
			arr.AddRange(rulesWithParentScopes);
			return SortBySpecificity(arr);
		}
		int dotIndex = scope.IndexOf('.');
		string head;
		string tail;
		if (dotIndex == -1)
		{
			head = scope;
			tail = "";
		}
		else
		{
			head = scope.SubstringAtIndexes(0, dotIndex);
			tail = scope.Substring(dotIndex + 1);
		}
		if (children.ContainsKey(head))
		{
			return children[head].Match(tail);
		}
		arr = new List<ThemeTrieElementRule>();
		if (mainRule.foreground > 0)
		{
			arr.Add(mainRule);
		}
		arr.AddRange(rulesWithParentScopes);
		return SortBySpecificity(arr);
	}

	public void Insert(string name, int scopeDepth, string scope, List<string> parentScopes, int fontStyle, int foreground, int background)
	{
		if ("".Equals(scope))
		{
			DoInsertHere(name, scopeDepth, parentScopes, fontStyle, foreground, background);
			return;
		}
		int dotIndex = scope.IndexOf('.');
		string head;
		string tail;
		if (dotIndex == -1)
		{
			head = scope;
			tail = "";
		}
		else
		{
			head = scope.SubstringAtIndexes(0, dotIndex);
			tail = scope.Substring(dotIndex + 1);
		}
		ThemeTrieElement child;
		if (children.ContainsKey(head))
		{
			child = children[head];
		}
		else
		{
			child = new ThemeTrieElement(mainRule.Clone(), ThemeTrieElementRule.cloneArr(rulesWithParentScopes));
			children[head] = child;
		}
		child.Insert(name, scopeDepth + 1, tail, parentScopes, fontStyle, foreground, background);
	}

	private void DoInsertHere(string name, int scopeDepth, List<string> parentScopes, int fontStyle, int foreground, int background)
	{
		if (parentScopes == null)
		{
			mainRule.AcceptOverwrite(name, scopeDepth, fontStyle, foreground, background);
			return;
		}
		foreach (ThemeTrieElementRule rule in rulesWithParentScopes)
		{
			if (StringUtils.StrArrCmp(rule.parentScopes, parentScopes) == 0)
			{
				rule.AcceptOverwrite(rule.name, scopeDepth, fontStyle, foreground, background);
				return;
			}
		}
		if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(mainRule.name))
		{
			name = mainRule.name;
		}
		if (fontStyle == -1)
		{
			fontStyle = mainRule.fontStyle;
		}
		if (foreground == 0)
		{
			foreground = mainRule.foreground;
		}
		if (background == 0)
		{
			background = mainRule.background;
		}
		rulesWithParentScopes.Add(new ThemeTrieElementRule(name, scopeDepth, parentScopes, fontStyle, foreground, background));
	}

	public override int GetHashCode()
	{
		return children.GetHashCode() + mainRule.GetHashCode() + rulesWithParentScopes.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (GetType() != obj.GetType())
		{
			return false;
		}
		ThemeTrieElement other = (ThemeTrieElement)obj;
		if (object.Equals(children, other.children) && object.Equals(mainRule, other.mainRule))
		{
			return object.Equals(rulesWithParentScopes, other.rulesWithParentScopes);
		}
		return false;
	}
}
