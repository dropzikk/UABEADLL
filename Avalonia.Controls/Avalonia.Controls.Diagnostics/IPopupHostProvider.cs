using System;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace Avalonia.Controls.Diagnostics;

[NotClientImplementable]
public interface IPopupHostProvider
{
	IPopupHost? PopupHost { get; }

	event Action<IPopupHost?>? PopupHostChanged;
}
