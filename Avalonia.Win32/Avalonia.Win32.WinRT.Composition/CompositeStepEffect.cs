using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class CompositeStepEffect : WinUIEffectBase
{
	private readonly float _mode;

	public override Guid EffectId => D2DEffects.CLSID_D2D1Composite;

	public override uint PropertyCount => 1u;

	public CompositeStepEffect(int mode, params IGraphicsEffectSource[] _sources)
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
