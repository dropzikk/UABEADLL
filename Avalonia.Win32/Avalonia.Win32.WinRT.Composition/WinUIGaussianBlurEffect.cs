using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class WinUIGaussianBlurEffect : WinUIEffectBase
{
	private enum D2D1_GAUSSIANBLUR_OPTIMIZATION
	{
		D2D1_GAUSSIANBLUR_OPTIMIZATION_SPEED,
		D2D1_GAUSSIANBLUR_OPTIMIZATION_BALANCED,
		D2D1_GAUSSIANBLUR_OPTIMIZATION_QUALITY,
		D2D1_GAUSSIANBLUR_OPTIMIZATION_FORCE_DWORD
	}

	private enum D2D1_BORDER_MODE
	{
		D2D1_BORDER_MODE_SOFT,
		D2D1_BORDER_MODE_HARD,
		D2D1_BORDER_MODE_FORCE_DWORD
	}

	private enum D2D1GaussianBlurProp
	{
		D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION,
		D2D1_GAUSSIANBLUR_PROP_OPTIMIZATION,
		D2D1_GAUSSIANBLUR_PROP_BORDER_MODE,
		D2D1_GAUSSIANBLUR_PROP_FORCE_DWORD
	}

	public override Guid EffectId => D2DEffects.CLSID_D2D1GaussianBlur;

	public override uint PropertyCount => 3u;

	public WinUIGaussianBlurEffect(IGraphicsEffectSource source)
		: base(source)
	{
	}

	public override IPropertyValue? GetProperty(uint index)
	{
		return (D2D1GaussianBlurProp)index switch
		{
			D2D1GaussianBlurProp.D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION => new WinRTPropertyValue(30f), 
			D2D1GaussianBlurProp.D2D1_GAUSSIANBLUR_PROP_OPTIMIZATION => new WinRTPropertyValue(1u), 
			D2D1GaussianBlurProp.D2D1_GAUSSIANBLUR_PROP_BORDER_MODE => new WinRTPropertyValue(1u), 
			_ => null, 
		};
	}
}
