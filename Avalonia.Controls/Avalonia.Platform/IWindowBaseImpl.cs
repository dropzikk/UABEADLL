using System;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IWindowBaseImpl : ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	double DesktopScaling { get; }

	PixelPoint Position { get; }

	Action<PixelPoint>? PositionChanged { get; set; }

	Action? Deactivated { get; set; }

	Action? Activated { get; set; }

	IPlatformHandle Handle { get; }

	Size MaxAutoSizeHint { get; }

	IScreenImpl Screen { get; }

	void Show(bool activate, bool isDialog);

	void Hide();

	void Activate();

	void SetTopmost(bool value);
}
