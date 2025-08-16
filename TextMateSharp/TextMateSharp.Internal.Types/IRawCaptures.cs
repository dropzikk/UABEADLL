using System.Collections;
using System.Collections.Generic;

namespace TextMateSharp.Internal.Types;

public interface IRawCaptures : IEnumerable<string>, IEnumerable
{
	IRawRule GetCapture(string captureId);
}
