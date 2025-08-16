using System;
using System.Collections.Generic;
using System.Diagnostics;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Matcher;
using TextMateSharp.Internal.Oniguruma;
using TextMateSharp.Internal.Rules;
using TextMateSharp.Internal.Utils;

namespace TextMateSharp.Internal.Grammars;

internal class LineTokenizer
{
	private class WhileStack
	{
		public StateStack Stack { get; private set; }

		public BeginWhileRule Rule { get; private set; }

		public WhileStack(StateStack stack, BeginWhileRule rule)
		{
			Stack = stack;
			Rule = rule;
		}
	}

	private class WhileCheckResult
	{
		public StateStack Stack { get; private set; }

		public int LinePos { get; private set; }

		public int AnchorPosition { get; private set; }

		public bool IsFirstLine { get; private set; }

		public WhileCheckResult(StateStack stack, int linePos, int anchorPosition, bool isFirstLine)
		{
			Stack = stack;
			LinePos = linePos;
			AnchorPosition = anchorPosition;
			IsFirstLine = isFirstLine;
		}
	}

	private Grammar _grammar;

	private string _lineText;

	private bool _isFirstLine;

	private int _linePos;

	private StateStack _stack;

	private LineTokens _lineTokens;

	private int _anchorPosition = -1;

	private bool _stop;

	private int _lineLength;

	public LineTokenizer(Grammar grammar, string lineText, bool isFirstLine, int linePos, StateStack stack, LineTokens lineTokens)
	{
		_grammar = grammar;
		_lineText = lineText;
		_lineLength = lineText.Length;
		_isFirstLine = isFirstLine;
		_linePos = linePos;
		_stack = stack;
		_lineTokens = lineTokens;
	}

	public TokenizeStringResult Scan(bool checkWhileConditions, TimeSpan timeLimit)
	{
		_stop = false;
		if (checkWhileConditions)
		{
			WhileCheckResult whileCheckResult = CheckWhileConditions(_grammar, _lineText, _isFirstLine, _linePos, _stack, _lineTokens);
			_stack = whileCheckResult.Stack;
			_linePos = whileCheckResult.LinePos;
			_isFirstLine = whileCheckResult.IsFirstLine;
			_anchorPosition = whileCheckResult.AnchorPosition;
		}
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();
		while (!_stop)
		{
			if (stopWatch.Elapsed > timeLimit)
			{
				return new TokenizeStringResult(_stack, stoppedEarly: true);
			}
			ScanNext();
		}
		return new TokenizeStringResult(_stack, stoppedEarly: false);
	}

	private void ScanNext()
	{
		MatchResult r = MatchRuleOrInjections(_grammar, _lineText, _isFirstLine, in _linePos, _stack, in _anchorPosition);
		if (r == null)
		{
			_lineTokens.Produce(_stack, _lineLength);
			_stop = true;
			return;
		}
		IOnigCaptureIndex[] captureIndices = r.CaptureIndexes;
		RuleId matchedRuleId = r.MatchedRuleId;
		bool hasAdvanced = captureIndices != null && captureIndices.Length != 0 && captureIndices[0].End > _linePos;
		if (matchedRuleId.Equals(RuleId.END_RULE))
		{
			BeginEndRule poppedRule = (BeginEndRule)_stack.GetRule(_grammar);
			_lineTokens.Produce(_stack, captureIndices[0].Start);
			_stack = _stack.WithContentNameScopesList(_stack.NameScopesList);
			HandleCaptures(_grammar, _lineText, _isFirstLine, _stack, _lineTokens, poppedRule.EndCaptures, captureIndices);
			_lineTokens.Produce(_stack, captureIndices[0].End);
			StateStack popped = _stack;
			_stack = _stack.Pop();
			_anchorPosition = popped.GetAnchorPos();
			if (!hasAdvanced && popped.GetEnterPos() == _linePos)
			{
				_stack = popped;
				_lineTokens.Produce(_stack, _lineLength);
				_stop = true;
				return;
			}
		}
		else if (captureIndices != null && captureIndices.Length != 0)
		{
			Rule rule = _grammar.GetRule(matchedRuleId);
			_lineTokens.Produce(_stack, captureIndices[0].Start);
			StateStack beforePush = _stack;
			string scopeName = rule.GetName(_lineText, captureIndices);
			AttributedScopeStack nameScopesList = _stack.ContentNameScopesList.PushAtributed(scopeName, _grammar);
			_stack = _stack.Push(matchedRuleId, _linePos, _anchorPosition, captureIndices[0].End == _lineText.Length, null, nameScopesList, nameScopesList);
			if (rule is BeginEndRule)
			{
				BeginEndRule pushedRule = (BeginEndRule)rule;
				HandleCaptures(_grammar, _lineText, _isFirstLine, _stack, _lineTokens, pushedRule.BeginCaptures, captureIndices);
				_lineTokens.Produce(_stack, captureIndices[0].End);
				_anchorPosition = captureIndices[0].End;
				string contentName = pushedRule.GetContentName(_lineText, captureIndices);
				AttributedScopeStack contentNameScopesList = nameScopesList.PushAtributed(contentName, _grammar);
				_stack = _stack.WithContentNameScopesList(contentNameScopesList);
				if (pushedRule.EndHasBackReferences)
				{
					_stack = _stack.WithEndRule(pushedRule.GetEndWithResolvedBackReferences(_lineText, captureIndices));
				}
				if (!hasAdvanced && beforePush.HasSameRuleAs(_stack))
				{
					_stack = _stack.Pop();
					_lineTokens.Produce(_stack, _lineLength);
					_stop = true;
					return;
				}
			}
			else if (rule is BeginWhileRule)
			{
				BeginWhileRule pushedRule2 = (BeginWhileRule)rule;
				HandleCaptures(_grammar, _lineText, _isFirstLine, _stack, _lineTokens, pushedRule2.BeginCaptures, captureIndices);
				_lineTokens.Produce(_stack, captureIndices[0].End);
				_anchorPosition = captureIndices[0].End;
				string contentName2 = pushedRule2.GetContentName(_lineText, captureIndices);
				AttributedScopeStack contentNameScopesList2 = nameScopesList.PushAtributed(contentName2, _grammar);
				_stack = _stack.WithContentNameScopesList(contentNameScopesList2);
				if (pushedRule2.WhileHasBackReferences)
				{
					_stack = _stack.WithEndRule(pushedRule2.getWhileWithResolvedBackReferences(_lineText, captureIndices));
				}
				if (!hasAdvanced && beforePush.HasSameRuleAs(_stack))
				{
					_stack = _stack.Pop();
					_lineTokens.Produce(_stack, _lineLength);
					_stop = true;
					return;
				}
			}
			else
			{
				MatchRule matchingRule = (MatchRule)rule;
				HandleCaptures(_grammar, _lineText, _isFirstLine, _stack, _lineTokens, matchingRule.Captures, captureIndices);
				_lineTokens.Produce(_stack, captureIndices[0].End);
				_stack = _stack.Pop();
				if (!hasAdvanced)
				{
					_stack = _stack.SafePop();
					_lineTokens.Produce(_stack, _lineLength);
					_stop = true;
					return;
				}
			}
		}
		if (captureIndices != null && captureIndices.Length != 0 && captureIndices[0].End > _linePos)
		{
			_linePos = captureIndices[0].End;
			_isFirstLine = false;
		}
	}

	private MatchResult MatchRule(Grammar grammar, string lineText, in bool isFirstLine, in int linePos, StateStack stack, in int anchorPosition)
	{
		Rule rule = stack.GetRule(grammar);
		if (rule == null)
		{
			return null;
		}
		CompiledRule ruleScanner = rule.Compile(grammar, stack.EndRule, isFirstLine, linePos == anchorPosition);
		if (ruleScanner == null)
		{
			return null;
		}
		IOnigNextMatchResult r = ruleScanner.Scanner.FindNextMatchSync(lineText, in linePos);
		if (r != null)
		{
			return new MatchResult(r.GetCaptureIndices(), ruleScanner.Rules[r.GetIndex()]);
		}
		return null;
	}

	private MatchResult MatchRuleOrInjections(Grammar grammar, string lineText, bool isFirstLine, in int linePos, StateStack stack, in int anchorPosition)
	{
		MatchResult matchResult = MatchRule(grammar, lineText, in isFirstLine, in linePos, stack, in anchorPosition);
		List<Injection> injections = grammar.GetInjections();
		if (injections.Count == 0)
		{
			return matchResult;
		}
		MatchInjectionsResult injectionResult = MatchInjections(injections, grammar, lineText, isFirstLine, in linePos, stack, in anchorPosition);
		if (injectionResult == null)
		{
			return matchResult;
		}
		if (matchResult == null)
		{
			return injectionResult;
		}
		int matchResultScore = matchResult.CaptureIndexes[0].Start;
		int injectionResultScore = injectionResult.CaptureIndexes[0].Start;
		if (injectionResultScore < matchResultScore || (injectionResult.IsPriorityMatch && injectionResultScore == matchResultScore))
		{
			return injectionResult;
		}
		return matchResult;
	}

	private MatchInjectionsResult MatchInjections(List<Injection> injections, Grammar grammar, string lineText, bool isFirstLine, in int linePos, StateStack stack, in int anchorPosition)
	{
		int bestMatchRating = int.MaxValue;
		IOnigCaptureIndex[] bestMatchCaptureIndices = null;
		RuleId bestMatchRuleId = null;
		int bestMatchResultPriority = 0;
		List<string> scopes = stack.ContentNameScopesList.GetScopeNames();
		foreach (Injection injection in injections)
		{
			if (!injection.Match(scopes))
			{
				continue;
			}
			CompiledRule ruleScanner = grammar.GetRule(injection.RuleId).Compile(grammar, null, isFirstLine, linePos == anchorPosition);
			IOnigNextMatchResult matchResult = ruleScanner.Scanner.FindNextMatchSync(lineText, in linePos);
			if (matchResult == null)
			{
				continue;
			}
			int matchRating = matchResult.GetCaptureIndices()[0].Start;
			if (matchRating <= bestMatchRating)
			{
				bestMatchRating = matchRating;
				bestMatchCaptureIndices = matchResult.GetCaptureIndices();
				bestMatchRuleId = ruleScanner.Rules[matchResult.GetIndex()];
				bestMatchResultPriority = injection.Priority;
				if (bestMatchRating == linePos)
				{
					break;
				}
			}
		}
		if (bestMatchCaptureIndices != null)
		{
			RuleId matchedRuleId = bestMatchRuleId;
			IOnigCaptureIndex[] captureIndexes = bestMatchCaptureIndices;
			bool isPriorityMatch = bestMatchResultPriority == -1;
			return new MatchInjectionsResult(captureIndexes, matchedRuleId, isPriorityMatch);
		}
		return null;
	}

	private void HandleCaptures(Grammar grammar, string lineText, bool isFirstLine, StateStack stack, LineTokens lineTokens, List<CaptureRule> captures, IOnigCaptureIndex[] captureIndices)
	{
		if (captures.Count == 0)
		{
			return;
		}
		int len = Math.Min(captures.Count, captureIndices.Length);
		List<LocalStackElement> localStack = new List<LocalStackElement>();
		int maxEnd = captureIndices[0].End;
		for (int i = 0; i < len; i++)
		{
			CaptureRule captureRule = captures[i];
			if (captureRule == null)
			{
				continue;
			}
			IOnigCaptureIndex captureIndex = captureIndices[i];
			if (captureIndex.Length == 0)
			{
				continue;
			}
			if (captureIndex.Start > maxEnd)
			{
				break;
			}
			while (localStack.Count > 0 && localStack[localStack.Count - 1].EndPos <= captureIndex.Start)
			{
				lineTokens.ProduceFromScopes(localStack[localStack.Count - 1].Scopes, localStack[localStack.Count - 1].EndPos);
				localStack.RemoveAt(localStack.Count - 1);
			}
			if (localStack.Count > 0)
			{
				lineTokens.ProduceFromScopes(localStack[localStack.Count - 1].Scopes, captureIndex.Start);
			}
			else
			{
				lineTokens.Produce(stack, captureIndex.Start);
			}
			if (captureRule.RetokenizeCapturedWithRuleId != null)
			{
				string scopeName = captureRule.GetName(lineText, captureIndices);
				AttributedScopeStack nameScopesList = stack.ContentNameScopesList.PushAtributed(scopeName, grammar);
				string contentName = captureRule.GetContentName(lineText, captureIndices);
				AttributedScopeStack contentNameScopesList = nameScopesList.PushAtributed(contentName, grammar);
				StateStack stackClone = stack.Push(captureRule.RetokenizeCapturedWithRuleId, captureIndex.Start, -1, beginRuleCapturedEOL: false, null, nameScopesList, contentNameScopesList);
				TokenizeString(grammar, lineText.SubstringAtIndexes(0, captureIndex.End), isFirstLine && captureIndex.Start == 0, captureIndex.Start, stackClone, lineTokens, checkWhileConditions: false, TimeSpan.MaxValue);
			}
			else
			{
				string captureRuleScopeName = captureRule.GetName(lineText, captureIndices);
				if (captureRuleScopeName != null)
				{
					AttributedScopeStack captureRuleScopesList = ((localStack.Count == 0) ? stack.ContentNameScopesList : localStack[localStack.Count - 1].Scopes).PushAtributed(captureRuleScopeName, grammar);
					localStack.Add(new LocalStackElement(captureRuleScopesList, captureIndex.End));
				}
			}
		}
		while (localStack.Count > 0)
		{
			lineTokens.ProduceFromScopes(localStack[localStack.Count - 1].Scopes, localStack[localStack.Count - 1].EndPos);
			localStack.RemoveAt(localStack.Count - 1);
		}
	}

	private WhileCheckResult CheckWhileConditions(Grammar grammar, string lineText, bool isFirstLine, int linePos, StateStack stack, LineTokens lineTokens)
	{
		int anchorPosition = ((!stack.BeginRuleCapturedEOL) ? (-1) : 0);
		List<WhileStack> whileRules = new List<WhileStack>();
		for (StateStack node = stack; node != null; node = node.Pop())
		{
			Rule nodeRule = node.GetRule(grammar);
			if (nodeRule is BeginWhileRule)
			{
				whileRules.Add(new WhileStack(node, (BeginWhileRule)nodeRule));
			}
		}
		int i = whileRules.Count - 1;
		while (i >= 0)
		{
			WhileStack whileRule = whileRules[i];
			CompiledRule ruleScanner = whileRule.Rule.CompileWhile(whileRule.Stack.EndRule, isFirstLine, anchorPosition == linePos);
			IOnigNextMatchResult r = ruleScanner.Scanner.FindNextMatchSync(lineText, in linePos);
			if (r != null)
			{
				RuleId matchedRuleId = ruleScanner.Rules[r.GetIndex()];
				if (RuleId.WHILE_RULE.NotEquals(matchedRuleId))
				{
					stack = whileRule.Stack.Pop();
					break;
				}
				if (r.GetCaptureIndices() != null && r.GetCaptureIndices().Length != 0)
				{
					lineTokens.Produce(whileRule.Stack, r.GetCaptureIndices()[0].Start);
					HandleCaptures(grammar, lineText, isFirstLine, whileRule.Stack, lineTokens, whileRule.Rule.WhileCaptures, r.GetCaptureIndices());
					lineTokens.Produce(whileRule.Stack, r.GetCaptureIndices()[0].End);
					anchorPosition = r.GetCaptureIndices()[0].End;
					if (r.GetCaptureIndices()[0].End > linePos)
					{
						linePos = r.GetCaptureIndices()[0].End;
						isFirstLine = false;
					}
				}
				i--;
				continue;
			}
			stack = whileRule.Stack.Pop();
			break;
		}
		return new WhileCheckResult(stack, linePos, anchorPosition, isFirstLine);
	}

	public static TokenizeStringResult TokenizeString(Grammar grammar, string lineText, bool isFirstLine, int linePos, StateStack stack, LineTokens lineTokens, bool checkWhileConditions, TimeSpan timeLimit)
	{
		return new LineTokenizer(grammar, lineText, isFirstLine, linePos, stack, lineTokens).Scan(checkWhileConditions, timeLimit);
	}
}
