using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class BlendEffect : WinUIEffectBase
{
	private readonly int _mode;

	public override Guid EffectId => D2DEffects.CLSID_D2D1Blend;

	public override uint PropertyCount => 1u;

	public BlendEffect(int mode, params IGraphicsEffectSource[] _sources)
		: base(_sources)
	{
		_mode = mode;
	}

	public override IPropertyValue? GetProperty(uint index)
	{
		if (index == 0)
		{
			return new WinRTPropertyValue((uint)_mode);
		}
		return null;
	}
}
