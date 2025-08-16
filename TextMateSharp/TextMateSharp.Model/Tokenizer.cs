using System;
using System.Collections.Generic;
using TextMateSharp.Grammars;

namespace TextMateSharp.Model;

public class Tokenizer : ITokenizationSupport
{
	private IGrammar _grammar;

	private DecodeMap _decodeMap;

	public Tokenizer(IGrammar grammar)
	{
		_grammar = grammar;
		_decodeMap = new DecodeMap();
	}

	public TMState GetInitialState()
	{
		return new TMState(null, null);
	}

	public LineTokens Tokenize(string line, TMState state, TimeSpan timeLimit)
	{
		return Tokenize(line, state, 0, 0, timeLimit);
	}

	public LineTokens Tokenize(string line, TMState state, int offsetDelta, int maxLen, TimeSpan timeLimit)
	{
		if (_grammar == null)
		{
			return null;
		}
		TMState freshState = ((state != null) ? state.Clone() : GetInitialState());
		if (line.Length > 0 && line.Length > maxLen)
		{
			line = line.Substring(0, maxLen);
		}
		ITokenizeLineResult textMateResult = _grammar.TokenizeLine(line, freshState.GetRuleStack(), timeLimit);
		freshState.SetRuleStack(textMateResult.RuleStack);
		List<TMToken> tokens = new List<TMToken>();
		string lastTokenType = null;
		IToken[] tmResultTokens = textMateResult.Tokens;
		int tokenIndex = 0;
		for (int len = tmResultTokens.Length; tokenIndex < len; tokenIndex++)
		{
			IToken token = tmResultTokens[tokenIndex];
			int tokenStartIndex = token.StartIndex;
			string tokenType = DecodeTextMateToken(_decodeMap, token.Scopes);
			if (!tokenType.Equals(lastTokenType))
			{
				tokens.Add(new TMToken(tokenStartIndex + offsetDelta, token.Scopes));
				lastTokenType = tokenType;
			}
		}
		return new LineTokens(tokens, offsetDelta + line.Length, freshState);
	}

	private string DecodeTextMateToken(DecodeMap decodeMap, List<string> scopes)
	{
		string[] prevTokenScopes = decodeMap.PrevToken.Scopes;
		int prevTokenScopesLength = prevTokenScopes.Length;
		Dictionary<int, Dictionary<int, bool>> prevTokenScopeTokensMaps = decodeMap.PrevToken.ScopeTokensMaps;
		Dictionary<int, Dictionary<int, bool>> scopeTokensMaps = new Dictionary<int, Dictionary<int, bool>>();
		Dictionary<int, bool> prevScopeTokensMaps = new Dictionary<int, bool>();
		bool sameAsPrev = true;
		for (int level = 1; level < scopes.Count; level++)
		{
			string scope = scopes[level];
			if (sameAsPrev)
			{
				if (level < prevTokenScopesLength && prevTokenScopes[level].Equals(scope))
				{
					prevScopeTokensMaps = (scopeTokensMaps[level] = prevTokenScopeTokensMaps[level]);
					continue;
				}
				sameAsPrev = false;
			}
			int[] tokenIds = decodeMap.getTokenIds(scope);
			prevScopeTokensMaps = new Dictionary<int, bool>(prevScopeTokensMaps);
			int[] array = tokenIds;
			foreach (int token in array)
			{
				prevScopeTokensMaps[token] = true;
			}
			scopeTokensMaps[level] = prevScopeTokensMaps;
		}
		decodeMap.PrevToken = new TMTokenDecodeData(scopes.ToArray(), scopeTokensMaps);
		return decodeMap.GetToken(prevScopeTokensMaps);
	}
}
