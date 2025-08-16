using System;
using System.Collections.Generic;
using System.Text;

namespace TextMateSharp.Model;

internal class DecodeMap
{
	private int lastAssignedId;

	private Dictionary<string, int[]> _scopeToTokenIds;

	private Dictionary<string, int?> _tokenToTokenId;

	private Dictionary<int, string> _tokenIdToToken;

	public TMTokenDecodeData PrevToken { get; set; }

	public DecodeMap()
	{
		PrevToken = new TMTokenDecodeData(new string[0], new Dictionary<int, Dictionary<int, bool>>());
		lastAssignedId = 0;
		_scopeToTokenIds = new Dictionary<string, int[]>();
		_tokenToTokenId = new Dictionary<string, int?>();
		_tokenIdToToken = new Dictionary<int, string>();
	}

	public int[] getTokenIds(string scope)
	{
		_scopeToTokenIds.TryGetValue(scope, out var tokens);
		if (tokens != null)
		{
			return tokens;
		}
		string[] tmpTokens = scope.Split(new string[1] { "[.]" }, StringSplitOptions.None);
		tokens = new int[tmpTokens.Length];
		for (int i = 0; i < tmpTokens.Length; i++)
		{
			string token = tmpTokens[i];
			_tokenToTokenId.TryGetValue(token, out var tokenId);
			if (!tokenId.HasValue)
			{
				tokenId = (lastAssignedId += 1);
				_tokenToTokenId[token] = tokenId.Value;
				_tokenIdToToken[tokenId.Value] = token;
			}
			tokens[i] = tokenId.Value;
		}
		_scopeToTokenIds[scope] = tokens;
		return tokens;
	}

	public string GetToken(Dictionary<int, bool> tokenMap)
	{
		StringBuilder result = new StringBuilder();
		bool isFirst = true;
		for (int i = 1; i <= lastAssignedId; i++)
		{
			if (tokenMap.ContainsKey(i))
			{
				if (isFirst)
				{
					isFirst = false;
					result.Append(_tokenIdToToken[i]);
				}
				else
				{
					result.Append(".");
					result.Append(_tokenIdToToken[i]);
				}
			}
		}
		return result.ToString();
	}
}
