using System;

namespace Avalonia.Win32.WinRT.Composition;

internal class BorderEffect : WinUIEffectBase
{
	private readonly int _x;

	private readonly int _y;

	public override Guid EffectId => D2DEffects.CLSID_D2D1Border;

	public override uint PropertyCount => 2u;

	public BorderEffect(int x, int y, params IGraphicsEffectSource[] _sources)
		: base(_sources)
	{
		_x = x;
		_y = y;
	}

	public override IPropertyValue? GetProperty(uint index)
	{
		return index switch
		{
			0u => new WinRTPropertyValue((uint)_x), 
			1u => new WinRTPropertyValue((uint)_y), 
			_ => null, 
		};
	}
}
