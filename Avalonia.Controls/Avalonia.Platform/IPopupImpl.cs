using System;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IPopupImpl : IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	IPopupPositioner? PopupPositioner { get; }

	void SetWindowManagerAddShadowHint(bool enabled);
}
