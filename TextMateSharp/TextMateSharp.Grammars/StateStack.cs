using System.Collections.Generic;
using TextMateSharp.Internal.Grammars;
using TextMateSharp.Internal.Rules;

namespace TextMateSharp.Grammars;

public class StateStack : IStateStack
{
	public static StateStack NULL = new StateStack(null, RuleId.NO_RULE, 0, 0, beginRuleCapturedEOL: false, null, null, null);

	private int _enterPos;

	private int _anchorPos;

	public StateStack Parent { get; private set; }

	public int Depth { get; private set; }

	public RuleId RuleId { get; private set; }

	public string EndRule { get; private set; }

	public AttributedScopeStack NameScopesList { get; private set; }

	public AttributedScopeStack ContentNameScopesList { get; private set; }

	public bool BeginRuleCapturedEOL { get; private set; }

	public StateStack(StateStack parent, RuleId ruleId, int enterPos, int anchorPos, bool beginRuleCapturedEOL, string endRule, AttributedScopeStack nameScopesList, AttributedScopeStack contentNameScopesList)
	{
		Parent = parent;
		Depth = ((Parent == null) ? 1 : (Parent.Depth + 1));
		RuleId = ruleId;
		BeginRuleCapturedEOL = beginRuleCapturedEOL;
		EndRule = endRule;
		NameScopesList = nameScopesList;
		ContentNameScopesList = contentNameScopesList;
		_enterPos = enterPos;
		_anchorPos = anchorPos;
	}

	private static bool StructuralEquals(StateStack a, StateStack b)
	{
		if (a == b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		if (a.Depth == b.Depth && a.RuleId == b.RuleId && object.Equals(a.EndRule, b.EndRule))
		{
			return StructuralEquals(a.Parent, b.Parent);
		}
		return false;
	}

	public override bool Equals(object other)
	{
		if (other == this)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		if (!(other is StateStack))
		{
			return false;
		}
		StateStack stackElement = (StateStack)other;
		if (StructuralEquals(this, stackElement))
		{
			return ContentNameScopesList.Equals(stackElement.ContentNameScopesList);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Depth.GetHashCode() + RuleId.GetHashCode() + EndRule.GetHashCode() + Parent.GetHashCode() + ContentNameScopesList.GetHashCode();
	}

	public void Reset()
	{
		for (StateStack el = this; el != null; el = el.Parent)
		{
			el._enterPos = -1;
		}
	}

	public StateStack Pop()
	{
		return Parent;
	}

	public StateStack SafePop()
	{
		if (Parent != null)
		{
			return Parent;
		}
		return this;
	}

	public StateStack Push(RuleId ruleId, int enterPos, int anchorPos, bool beginRuleCapturedEOL, string endRule, AttributedScopeStack nameScopesList, AttributedScopeStack contentNameScopesList)
	{
		return new StateStack(this, ruleId, enterPos, anchorPos, beginRuleCapturedEOL, endRule, nameScopesList, contentNameScopesList);
	}

	public int GetEnterPos()
	{
		return _enterPos;
	}

	public int GetAnchorPos()
	{
		return _anchorPos;
	}

	public Rule GetRule(IRuleRegistry grammar)
	{
		return grammar.GetRule(RuleId);
	}

	private void AppendString(List<string> res)
	{
		if (Parent != null)
		{
			Parent.AppendString(res);
		}
		res.Add("(" + RuleId.ToString() + ")");
	}

	public override string ToString()
	{
		List<string> r = new List<string>();
		AppendString(r);
		return "[" + string.Join(", ", r) + "]";
	}

	public StateStack WithContentNameScopesList(AttributedScopeStack contentNameScopesList)
	{
		if (ContentNameScopesList.Equals(contentNameScopesList))
		{
			return this;
		}
		return Parent.Push(RuleId, _enterPos, _anchorPos, BeginRuleCapturedEOL, EndRule, NameScopesList, contentNameScopesList);
	}

	public StateStack WithEndRule(string endRule)
	{
		if (EndRule != null && EndRule.Equals(endRule))
		{
			return this;
		}
		return new StateStack(Parent, RuleId, _enterPos, _anchorPos, BeginRuleCapturedEOL, endRule, NameScopesList, ContentNameScopesList);
	}

	public bool HasSameRuleAs(StateStack other)
	{
		return RuleId == other.RuleId;
	}
}
