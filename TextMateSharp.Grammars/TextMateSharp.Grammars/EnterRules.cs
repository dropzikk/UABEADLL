using System.Collections.Generic;

namespace TextMateSharp.Grammars;

public class EnterRules
{
	public IList<EnterRule> Rules { get; set; } = new List<EnterRule>();
}
