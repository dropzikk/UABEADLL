using TextMateSharp.Grammars;

namespace TextMateSharp.Model;

public class TMState
{
	private TMState _parentEmbedderState;

	private IStateStack _ruleStack;

	public TMState(TMState parentEmbedderState, IStateStack ruleStatck)
	{
		_parentEmbedderState = parentEmbedderState;
		_ruleStack = ruleStatck;
	}

	public void SetRuleStack(IStateStack ruleStack)
	{
		_ruleStack = ruleStack;
	}

	public IStateStack GetRuleStack()
	{
		return _ruleStack;
	}

	public TMState Clone()
	{
		return new TMState((_parentEmbedderState != null) ? _parentEmbedderState.Clone() : null, _ruleStack);
	}

	public override bool Equals(object other)
	{
		if (!(other is TMState))
		{
			return false;
		}
		TMState otherState = (TMState)other;
		if (object.Equals(_parentEmbedderState, otherState._parentEmbedderState))
		{
			return object.Equals(_ruleStack, otherState._ruleStack);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _parentEmbedderState.GetHashCode() + _ruleStack.GetHashCode();
	}
}
