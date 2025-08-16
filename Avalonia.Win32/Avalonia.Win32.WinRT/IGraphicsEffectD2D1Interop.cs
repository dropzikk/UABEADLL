using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT;

internal interface IGraphicsEffectD2D1Interop : IUnknown, IDisposable
{
	Guid EffectId { get; }

	uint PropertyCount { get; }

	uint SourceCount { get; }

	unsafe void GetNamedPropertyMapping(IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping);

	IPropertyValue GetProperty(uint index);

	IGraphicsEffectSource GetSource(uint index);
}
