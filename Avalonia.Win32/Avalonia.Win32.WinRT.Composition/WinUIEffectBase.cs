using System;
using System.Linq;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Composition;

internal abstract class WinUIEffectBase : WinRTInspectable, IGraphicsEffect, IInspectable, IUnknown, IDisposable, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
{
	private IGraphicsEffectSource[]? _sources;

	public IntPtr Name => IntPtr.Zero;

	public abstract Guid EffectId { get; }

	public abstract uint PropertyCount { get; }

	public uint SourceCount
	{
		get
		{
			IGraphicsEffectSource[]? sources = _sources;
			if (sources == null)
			{
				return 0u;
			}
			return (uint)sources.Length;
		}
	}

	public WinUIEffectBase(params IGraphicsEffectSource[] _sources)
	{
		this._sources = _sources.Select((IGraphicsEffectSource e) => (e is WinUIEffectBase) ? e : e.CloneReference()).ToArray();
	}

	public void SetName(IntPtr name)
	{
	}

	public unsafe void GetNamedPropertyMapping(IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
	{
		throw new COMException("Not supported", -2147467263);
	}

	public abstract IPropertyValue? GetProperty(uint index);

	public IGraphicsEffectSource GetSource(uint index)
	{
		if (_sources == null || index > _sources.Length)
		{
			throw new COMException("Invalid index", -2147024809);
		}
		return _sources[index];
	}

	public override void OnUnreferencedFromNative()
	{
		if (_sources != null)
		{
			_sources = null;
		}
	}
}
