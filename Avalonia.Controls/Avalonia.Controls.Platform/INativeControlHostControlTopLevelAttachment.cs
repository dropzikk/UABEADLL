using System;
using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface INativeControlHostControlTopLevelAttachment : IDisposable
{
	INativeControlHostImpl? AttachedTo { get; set; }

	bool IsCompatibleWith(INativeControlHostImpl host);

	void HideWithSize(Size size);

	void ShowInBounds(Rect rect);
}
