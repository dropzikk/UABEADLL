using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Styling;

[NotClientImplementable]
public interface IStyleHost
{
	bool IsStylesInitialized { get; }

	Styles Styles { get; }

	IStyleHost? StylingParent { get; }

	void StylesAdded(IReadOnlyList<IStyle> styles);

	void StylesRemoved(IReadOnlyList<IStyle> styles);
}
