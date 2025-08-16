using System.Collections.Generic;
using TextMateSharp.Grammars;

namespace TextMateSharp.Internal.Grammars;

internal class LineTokens
{
	private string _lineText;

	private List<IToken> _tokens;

	private bool _emitBinaryTokens;

	private List<int> binaryTokens;

	private int _lastTokenEndIndex;

	private List<TokenTypeMatcher> _tokenTypeOverrides;

	private BalancedBracketSelectors _balancedBracketSelectors;

	internal LineTokens(bool emitBinaryTokens, string lineText, List<TokenTypeMatcher> tokenTypeOverrides, BalancedBracketSelectors balancedBracketSelectors)
	{
		_emitBinaryTokens = emitBinaryTokens;
		_lineText = lineText;
		if (_emitBinaryTokens)
		{
			_tokens = null;
			binaryTokens = new List<int>();
		}
		else
		{
			_tokens = new List<IToken>();
			binaryTokens = null;
		}
		_tokenTypeOverrides = tokenTypeOverrides;
		_balancedBracketSelectors = balancedBracketSelectors;
	}

	public void Produce(StateStack stack, int endIndex)
	{
		ProduceFromScopes(stack.ContentNameScopesList, endIndex);
	}

	public void ProduceFromScopes(AttributedScopeStack scopesList, int endIndex)
	{
		if (_lastTokenEndIndex >= endIndex)
		{
			return;
		}
		if (_emitBinaryTokens)
		{
			int metadata = scopesList.TokenAttributes;
			bool containsBalancedBrackets = false;
			BalancedBracketSelectors balancedBracketSelectors = _balancedBracketSelectors;
			if (balancedBracketSelectors != null && balancedBracketSelectors.MatchesAlways())
			{
				containsBalancedBrackets = true;
			}
			if (_tokenTypeOverrides.Count > 0 || (balancedBracketSelectors != null && !balancedBracketSelectors.MatchesAlways() && !balancedBracketSelectors.MatchesNever()))
			{
				List<string> scopes2 = scopesList.GetScopeNames();
				foreach (TokenTypeMatcher tokenType in _tokenTypeOverrides)
				{
					if (tokenType.Matcher(scopes2))
					{
						metadata = EncodedTokenAttributes.Set(metadata, 0, tokenType.Type, null, -1, 0, 0);
					}
				}
				if (balancedBracketSelectors != null)
				{
					containsBalancedBrackets = balancedBracketSelectors.Match(scopes2);
				}
			}
			if (containsBalancedBrackets)
			{
				metadata = EncodedTokenAttributes.Set(metadata, 0, OptionalStandardTokenType.NotSet, containsBalancedBrackets, -1, 0, 0);
			}
			if (binaryTokens.Count != 0 && binaryTokens[binaryTokens.Count - 1] == metadata)
			{
				_lastTokenEndIndex = endIndex;
				return;
			}
			binaryTokens.Add(_lastTokenEndIndex);
			binaryTokens.Add(metadata);
			_lastTokenEndIndex = endIndex;
		}
		else
		{
			List<string> scopes3 = scopesList.GetScopeNames();
			_tokens.Add(new Token((_lastTokenEndIndex >= 0) ? _lastTokenEndIndex : 0, endIndex, scopes3));
			_lastTokenEndIndex = endIndex;
		}
	}

	public IToken[] GetResult(StateStack stack, int lineLength)
	{
		if (_tokens.Count != 0 && _tokens[_tokens.Count - 1].StartIndex == lineLength - 1)
		{
			_tokens.RemoveAt(_tokens.Count - 1);
		}
		if (_tokens.Count == 0)
		{
			_lastTokenEndIndex = -1;
			Produce(stack, lineLength);
			_tokens[_tokens.Count - 1].StartIndex = 0;
		}
		return _tokens.ToArray();
	}

	public int[] GetBinaryResult(StateStack stack, int lineLength)
	{
		if (binaryTokens.Count != 0 && binaryTokens[binaryTokens.Count - 2] == lineLength - 1)
		{
			binaryTokens.RemoveAt(binaryTokens.Count - 1);
			binaryTokens.RemoveAt(binaryTokens.Count - 1);
		}
		if (binaryTokens.Count == 0)
		{
			_lastTokenEndIndex = -1;
			Produce(stack, lineLength);
			binaryTokens[binaryTokens.Count - 2] = 0;
		}
		int[] result = new int[binaryTokens.Count];
		int i = 0;
		for (int len = binaryTokens.Count; i < len; i++)
		{
			result[i] = binaryTokens[i];
		}
		return result;
	}
}
