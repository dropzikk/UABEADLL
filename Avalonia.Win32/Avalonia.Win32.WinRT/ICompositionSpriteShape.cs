using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface ICompositionSpriteShape : IInspectable, IUnknown, IDisposable
{
	ICompositionBrush FillBrush { get; }

	ICompositionGeometry Geometry { get; }

	int IsStrokeNonScaling { get; }

	ICompositionBrush StrokeBrush { get; }

	void SetFillBrush(ICompositionBrush value);

	void SetGeometry(ICompositionGeometry value);

	void SetIsStrokeNonScaling(int value);

	void SetStrokeBrush(ICompositionBrush value);

	void GetStrokeDashArray();

	void GetStrokeDashCap();

	void SetStrokeDashCap();

	void GetStrokeDashOffset();

	void SetStrokeDashOffset();

	void GetStrokeEndCap();

	void SetStrokeEndCap();

	void GetStrokeLineJoin();

	void SetStrokeLineJoin();

	void GetStrokeMiterLimit();

	void SetStrokeMiterLimit();

	void GetStrokeStartCap();

	void SetStrokeStartCap();

	void GetStrokeThickness();

	void SetStrokeThickness();
}
