using System;
using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Styling;

[NotClientImplementable]
public interface IGlobalStyles : IStyleHost
{
	event Action<IReadOnlyList<IStyle>>? GlobalStylesAdded;

	event Action<IReadOnlyList<IStyle>>? GlobalStylesRemoved;
}
