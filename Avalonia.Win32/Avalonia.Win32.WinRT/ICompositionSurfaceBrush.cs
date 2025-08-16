using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionSurfaceBrush : IInspectable, IUnknown, IDisposable
{
	CompositionBitmapInterpolationMode BitmapInterpolationMode { get; }

	float HorizontalAlignmentRatio { get; }

	CompositionStretch Stretch { get; }

	ICompositionSurface Surface { get; }

	float VerticalAlignmentRatio { get; }

	void SetBitmapInterpolationMode(CompositionBitmapInterpolationMode value);

	void SetHorizontalAlignmentRatio(float value);

	void SetStretch(CompositionStretch value);

	void SetSurface(ICompositionSurface value);

	void SetVerticalAlignmentRatio(float value);
}
