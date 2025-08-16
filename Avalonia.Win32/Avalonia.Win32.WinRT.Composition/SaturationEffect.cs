using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class SaturationEffect : WinUIEffectBase
{
	private enum D2D1_SATURATION_PROP
	{
		D2D1_SATURATION_PROP_SATURATION,
		D2D1_SATURATION_PROP_FORCE_DWORD
	}

	public override Guid EffectId => D2DEffects.CLSID_D2D1Saturation;

	public override uint PropertyCount => 1u;

	public SaturationEffect(IGraphicsEffectSource source)
		: base(source)
	{
	}

	public override IPropertyValue? GetProperty(uint index)
	{
		if (index == 0)
		{
			return new WinRTPropertyValue(2f);
		}
		return null;
	}
}
