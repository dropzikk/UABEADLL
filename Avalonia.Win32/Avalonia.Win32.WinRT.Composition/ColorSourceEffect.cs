using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class ColorSourceEffect : WinUIEffectBase
{
	private readonly float[] _color;

	public override Guid EffectId => D2DEffects.CLSID_D2D1Flood;

	public override uint PropertyCount => 1u;

	public ColorSourceEffect(float[] color)
	{
		_color = color;
	}

	public override IPropertyValue? GetProperty(uint index)
	{
		if (index == 0)
		{
			return new WinRTPropertyValue(_color);
		}
		return null;
	}
}
