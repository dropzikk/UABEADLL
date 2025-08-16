using System;
using Avalonia.Controls.Platform;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface ITrayIconImpl : IDisposable
{
	INativeMenuExporter? MenuExporter { get; }

	Action? OnClicked { get; set; }

	void SetIcon(IWindowIconImpl? icon);

	void SetToolTipText(string? text);

	void SetIsVisible(bool visible);
}
