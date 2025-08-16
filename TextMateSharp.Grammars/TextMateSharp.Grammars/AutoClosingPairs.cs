using System.Collections.Generic;

namespace TextMateSharp.Grammars;

public class AutoClosingPairs
{
	public IList<char>[] CharPairs { get; set; } = new IList<char>[0];

	public AutoPair[] AutoPairs { get; set; } = new AutoPair[0];
}
