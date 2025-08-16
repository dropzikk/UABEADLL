using System.Collections.Generic;

namespace TextMateSharp.Internal.Matcher;

public interface IMatchesName<T>
{
	bool Match(ICollection<string> names, T scopes);
}
