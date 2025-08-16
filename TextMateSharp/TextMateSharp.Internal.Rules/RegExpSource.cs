using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TextMateSharp.Internal.Oniguruma;
using TextMateSharp.Internal.Utils;

namespace TextMateSharp.Internal.Rules;

public class RegExpSource
{
	private static Regex HAS_BACK_REFERENCES = new Regex("\\\\(\\d+)");

	private static Regex BACK_REFERENCING_END = new Regex("\\\\(\\d+)");

	private RuleId _ruleId;

	private bool _hasAnchor;

	private bool _hasBackReferences;

	private RegExpSourceAnchorCache _anchorCache;

	private string _source;

	public RegExpSource(string regExpSource, RuleId ruleId)
		: this(regExpSource, ruleId, handleAnchors: true)
	{
	}

	public RegExpSource(string regExpSource, RuleId ruleId, bool handleAnchors)
	{
		if (handleAnchors)
		{
			HandleAnchors(regExpSource);
		}
		else
		{
			_source = regExpSource;
			_hasAnchor = false;
		}
		if (_hasAnchor)
		{
			_anchorCache = BuildAnchorCache();
		}
		_ruleId = ruleId;
		_hasBackReferences = HAS_BACK_REFERENCES.Match(_source).Success;
	}

	public RegExpSource Clone()
	{
		return new RegExpSource(_source, _ruleId, handleAnchors: true);
	}

	public void SetSource(string newSource)
	{
		if (!_source.Equals(newSource))
		{
			_source = newSource;
			if (_hasAnchor)
			{
				_anchorCache = BuildAnchorCache();
			}
		}
	}

	private void HandleAnchors(string regExpSource)
	{
		if (regExpSource != null)
		{
			int len = regExpSource.Length;
			int lastPushedPos = 0;
			StringBuilder output = new StringBuilder();
			bool hasAnchor = false;
			for (int pos = 0; pos < len; pos++)
			{
				if (regExpSource[pos] == '\\' && pos + 1 < len)
				{
					switch (regExpSource[pos + 1])
					{
					case 'z':
						output.Append(regExpSource.SubstringAtIndexes(lastPushedPos, pos));
						output.Append("$(?!\\n)(?<!\\n)");
						lastPushedPos = pos + 2;
						break;
					case 'A':
					case 'G':
						hasAnchor = true;
						break;
					}
					pos++;
				}
			}
			_hasAnchor = hasAnchor;
			if (lastPushedPos == 0)
			{
				_source = regExpSource;
				return;
			}
			output.Append(regExpSource.SubstringAtIndexes(lastPushedPos, len));
			_source = output.ToString();
		}
		else
		{
			_hasAnchor = false;
			_source = regExpSource;
		}
	}

	public string ResolveBackReferences(string lineText, IOnigCaptureIndex[] captureIndices)
	{
		List<string> capturedValues = new List<string>();
		try
		{
			foreach (IOnigCaptureIndex captureIndex in captureIndices)
			{
				capturedValues.Add(lineText.SubstringAtIndexes(captureIndex.Start, captureIndex.End));
			}
			return BACK_REFERENCING_END.Replace(_source, delegate(Match m)
			{
				try
				{
					string value = m.Value;
					int num = int.Parse(m.Value.SubstringAtIndexes(1, value.Length));
					return EscapeRegExpCharacters((capturedValues.Count > num) ? capturedValues[num] : "");
				}
				catch (Exception)
				{
					return "";
				}
			});
		}
		catch (Exception)
		{
			return lineText;
		}
	}

	private string EscapeRegExpCharacters(string value)
	{
		int valueLen = value.Length;
		StringBuilder sb = new StringBuilder(valueLen);
		for (int i = 0; i < valueLen; i++)
		{
			char ch = value[i];
			switch (ch)
			{
			case '#':
			case '$':
			case '(':
			case ')':
			case '*':
			case '+':
			case ',':
			case '-':
			case '.':
			case '?':
			case '[':
			case '\\':
			case ']':
			case '^':
			case '{':
			case '|':
			case '}':
				sb.Append('\\');
				break;
			}
			sb.Append(ch);
		}
		return sb.ToString();
	}

	private RegExpSourceAnchorCache BuildAnchorCache()
	{
		string source = _source;
		int length = source.Length;
		StringBuilder A0_G0_result = new StringBuilder(length);
		StringBuilder A0_G1_result = new StringBuilder(length);
		StringBuilder A1_G0_result = new StringBuilder(length);
		StringBuilder A1_G1_result = new StringBuilder(length);
		int pos = 0;
		for (int len = length; pos < len; pos++)
		{
			char ch = source[pos];
			A0_G0_result.Append(ch);
			A0_G1_result.Append(ch);
			A1_G0_result.Append(ch);
			A1_G1_result.Append(ch);
			if (ch == '\\' && pos + 1 < len)
			{
				char nextCh = source[pos + 1];
				switch (nextCh)
				{
				case 'A':
					A0_G0_result.Append('\uffff');
					A0_G1_result.Append('\uffff');
					A1_G0_result.Append('A');
					A1_G1_result.Append('A');
					break;
				case 'G':
					A0_G0_result.Append('\uffff');
					A0_G1_result.Append('G');
					A1_G0_result.Append('\uffff');
					A1_G1_result.Append('G');
					break;
				default:
					A0_G0_result.Append(nextCh);
					A0_G1_result.Append(nextCh);
					A1_G0_result.Append(nextCh);
					A1_G1_result.Append(nextCh);
					break;
				}
				pos++;
			}
		}
		return new RegExpSourceAnchorCache(A0_G0_result.ToString(), A0_G1_result.ToString(), A1_G0_result.ToString(), A1_G1_result.ToString());
	}

	public string ResolveAnchors(bool allowA, bool allowG)
	{
		if (!_hasAnchor)
		{
			return _source;
		}
		if (allowA)
		{
			if (allowG)
			{
				return _anchorCache.A1_G1;
			}
			return _anchorCache.A1_G0;
		}
		if (allowG)
		{
			return _anchorCache.A0_G1;
		}
		return _anchorCache.A0_G0;
	}

	public bool HasAnchor()
	{
		return _hasAnchor;
	}

	public string GetSource()
	{
		return _source;
	}

	public RuleId GetRuleId()
	{
		return _ruleId;
	}

	public bool HasBackReferences()
	{
		return _hasBackReferences;
	}
}
