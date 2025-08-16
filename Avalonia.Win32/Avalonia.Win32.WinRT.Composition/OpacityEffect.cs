using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class OpacityEffect : WinUIEffectBase
{
	private readonly float _opacity;

	public override Guid EffectId => D2DEffects.CLSID_D2D1Opacity;

	public override uint PropertyCount => 1u;

	public OpacityEffect(float opacity, params IGraphicsEffectSource[] _sources)
		: base(_sources)
	{
		_opacity = opacity;
	}

	public override IPropertyValue? GetProperty(uint index)
	{
		if (index == 0)
		{
			return new WinRTPropertyValue(_opacity);
		}
		return null;
	}
}
