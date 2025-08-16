using System.Collections.Generic;

namespace TextMateSharp.Grammars;

public class AutoPair
{
	public string Open { get; set; }

	public string Close { get; set; }

	public IList<string> NotIn { get; set; }
}
