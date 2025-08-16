using System;
using System.Collections.Generic;
using TextMateSharp.Themes;

namespace TextMateSharp.Internal.Grammars;

public class AttributedScopeStack
{
	public AttributedScopeStack Parent { get; private set; }

	public string ScopePath { get; private set; }

	public int TokenAttributes { get; private set; }

	public AttributedScopeStack(AttributedScopeStack parent, string scopePath, int tokenAttributes)
	{
		Parent = parent;
		ScopePath = scopePath;
		TokenAttributes = tokenAttributes;
	}

	private static bool StructuralEquals(AttributedScopeStack a, AttributedScopeStack b)
	{
		while (true)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null && b == null)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.ScopePath != b.ScopePath || a.TokenAttributes != b.TokenAttributes)
			{
				break;
			}
			a = a.Parent;
			b = b.Parent;
		}
		return false;
	}

	private static bool Equals(AttributedScopeStack a, AttributedScopeStack b)
	{
		if (a == b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		return StructuralEquals(a, b);
	}

	public override bool Equals(object other)
	{
		if (other == null || other is AttributedScopeStack)
		{
			return false;
		}
		return Equals(this, (AttributedScopeStack)other);
	}

	public override int GetHashCode()
	{
		return Parent.GetHashCode() + ScopePath.GetHashCode() + TokenAttributes.GetHashCode();
	}

	private static bool MatchesScope(string scope, string selector, string selectorWithDot)
	{
		if (!selector.Equals(scope))
		{
			return scope.StartsWith(selectorWithDot);
		}
		return true;
	}

	private static bool Matches(AttributedScopeStack target, List<string> parentScopes)
	{
		if (parentScopes == null)
		{
			return true;
		}
		int len = parentScopes.Count;
		int index = 0;
		string selector = parentScopes[index];
		string selectorWithDot = selector + ".";
		while (target != null)
		{
			if (MatchesScope(target.ScopePath, selector, selectorWithDot))
			{
				index++;
				if (index == len)
				{
					return true;
				}
				selector = parentScopes[index];
				selectorWithDot = selector + ".";
			}
			target = target.Parent;
		}
		return false;
	}

	public static int MergeAttributes(int existingTokenAttributes, AttributedScopeStack scopesList, BasicScopeAttributes basicScopeAttributes)
	{
		if (basicScopeAttributes == null)
		{
			return existingTokenAttributes;
		}
		int fontStyle = -1;
		int foreground = 0;
		int background = 0;
		if (basicScopeAttributes.ThemeData != null)
		{
			foreach (ThemeTrieElementRule themeData in basicScopeAttributes.ThemeData)
			{
				if (Matches(scopesList, themeData.parentScopes))
				{
					fontStyle = themeData.fontStyle;
					foreground = themeData.foreground;
					background = themeData.background;
					break;
				}
			}
		}
		return EncodedTokenAttributes.Set(existingTokenAttributes, basicScopeAttributes.LanguageId, basicScopeAttributes.TokenType, null, fontStyle, foreground, background);
	}

	private static AttributedScopeStack Push(AttributedScopeStack target, Grammar grammar, List<string> scopes)
	{
		foreach (string scope in scopes)
		{
			BasicScopeAttributes rawMetadata = grammar.GetMetadataForScope(scope);
			int metadata = MergeAttributes(target.TokenAttributes, target, rawMetadata);
			target = new AttributedScopeStack(target, scope, metadata);
		}
		return target;
	}

	public AttributedScopeStack PushAtributed(string scopePath, Grammar grammar)
	{
		if (scopePath == null)
		{
			return this;
		}
		if (scopePath.IndexOf(' ') >= 0)
		{
			return Push(this, grammar, new List<string>(scopePath.Split(new string[1] { " " }, StringSplitOptions.None)));
		}
		return Push(this, grammar, new List<string> { scopePath });
	}

	public List<string> GetScopeNames()
	{
		return GenerateScopes(this);
	}

	private static List<string> GenerateScopes(AttributedScopeStack scopesList)
	{
		List<string> result = new List<string>();
		while (scopesList != null)
		{
			result.Add(scopesList.ScopePath);
			scopesList = scopesList.Parent;
		}
		result.Reverse();
		return result;
	}
}
