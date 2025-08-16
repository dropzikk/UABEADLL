using TextMateSharp.Internal.Rules;

namespace TextMateSharp.Grammars;

public interface IStateStack
{
	int Depth { get; }

	RuleId RuleId { get; }

	string EndRule { get; }
}
