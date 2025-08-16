using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextMateSharp.Internal.Matcher;

public class MatcherBuilder<T>
{
	private class Tokenizer
	{
		private static Regex REGEXP = new Regex("([LR]:|[\\w\\.:][\\w\\.:\\-]*|[\\,\\|\\-\\(\\)])");

		private string _input;

		private Match _currentMatch;

		public Tokenizer(string input)
		{
			_input = input;
		}

		public string Next()
		{
			if (_currentMatch == null)
			{
				_currentMatch = REGEXP.Match(_input);
			}
			else
			{
				_currentMatch = _currentMatch.NextMatch();
			}
			if (_currentMatch.Success)
			{
				return _currentMatch.Value;
			}
			return null;
		}
	}

	public List<MatcherWithPriority<T>> Results;

	private Tokenizer _tokenizer;

	private IMatchesName<T> _matchesName;

	private string _token;

	public MatcherBuilder(string expression, IMatchesName<T> matchesName)
	{
		Results = new List<MatcherWithPriority<T>>();
		_tokenizer = new Tokenizer(expression);
		_matchesName = matchesName;
		_token = _tokenizer.Next();
		while (_token != null)
		{
			int priority = 0;
			if (_token.Length == 2 && _token[1] == ':')
			{
				switch (_token[0])
				{
				case 'R':
					priority = 1;
					break;
				case 'L':
					priority = -1;
					break;
				}
				_token = _tokenizer.Next();
			}
			Predicate<T> matcher = ParseConjunction();
			if (matcher != null)
			{
				Results.Add(new MatcherWithPriority<T>(matcher, priority));
			}
			if (",".Equals(_token))
			{
				_token = _tokenizer.Next();
				continue;
			}
			break;
		}
	}

	private Predicate<T> ParseInnerExpression()
	{
		List<Predicate<T>> matchers = new List<Predicate<T>>();
		for (Predicate<T> matcher = ParseConjunction(); matcher != null; matcher = ParseConjunction())
		{
			matchers.Add(matcher);
			if (!"|".Equals(_token) && !",".Equals(_token))
			{
				break;
			}
			do
			{
				_token = _tokenizer.Next();
			}
			while ("|".Equals(_token) || ",".Equals(_token));
		}
		return delegate(T matcherInput)
		{
			foreach (Predicate<T> item in matchers)
			{
				if (item(matcherInput))
				{
					return true;
				}
			}
			return false;
		};
	}

	private Predicate<T> ParseConjunction()
	{
		List<Predicate<T>> matchers = new List<Predicate<T>>();
		for (Predicate<T> matcher = ParseOperand(); matcher != null; matcher = ParseOperand())
		{
			matchers.Add(matcher);
		}
		return delegate(T matcherInput)
		{
			foreach (Predicate<T> item in matchers)
			{
				if (!item(matcherInput))
				{
					return false;
				}
			}
			return true;
		};
	}

	private Predicate<T> ParseOperand()
	{
		if ("-".Equals(_token))
		{
			_token = _tokenizer.Next();
			Predicate<T> expressionToNegate = ParseOperand();
			return (T matcherInput) => expressionToNegate != null && !expressionToNegate(matcherInput);
		}
		if ("(".Equals(_token))
		{
			_token = _tokenizer.Next();
			Predicate<T> result = ParseInnerExpression();
			if (")".Equals(_token))
			{
				_token = _tokenizer.Next();
			}
			return result;
		}
		if (IsIdentifier(_token))
		{
			ICollection<string> identifiers = new List<string>();
			do
			{
				identifiers.Add(_token);
				_token = _tokenizer.Next();
			}
			while (_token != null && IsIdentifier(_token));
			return (T matcherInput) => _matchesName.Match(identifiers, matcherInput);
		}
		return null;
	}

	private bool IsIdentifier(string token)
	{
		if (string.IsNullOrEmpty(token))
		{
			return false;
		}
		foreach (char ch in token)
		{
			switch (ch)
			{
			case '.':
			case ':':
			case '_':
			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
			case 'g':
			case 'h':
			case 'i':
			case 'j':
			case 'k':
			case 'l':
			case 'm':
			case 'n':
			case 'o':
			case 'p':
			case 'q':
			case 'r':
			case 's':
			case 't':
			case 'u':
			case 'v':
			case 'w':
			case 'x':
			case 'y':
			case 'z':
				continue;
			}
			if ((ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9'))
			{
				return false;
			}
		}
		return true;
	}
}
