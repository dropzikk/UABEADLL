using System;
using System.IO;
using Avalonia.Controls.Platform;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class TrayIconImpl : ITrayIconImpl, IDisposable
{
	private readonly IAvnTrayIcon _native;

	public Action? OnClicked { get; set; }

	public INativeMenuExporter? MenuExporter { get; }

	public TrayIconImpl(IAvaloniaNativeFactory factory)
	{
		_native = factory.CreateTrayIcon();
		MenuExporter = new AvaloniaNativeMenuExporter(_native, factory);
	}

	public void Dispose()
	{
		_native.Dispose();
	}

	public unsafe void SetIcon(IWindowIconImpl? icon)
	{
		if (icon == null)
		{
			_native.SetIcon(null, IntPtr.Zero);
			return;
		}
		using MemoryStream memoryStream = new MemoryStream();
		icon.Save(memoryStream);
		byte[] array = memoryStream.ToArray();
		fixed (byte* ptr = array)
		{
			void* data = ptr;
			_native.SetIcon(data, new IntPtr(array.Length));
		}
	}

	public void SetToolTipText(string? text)
	{
	}

	public void SetIsVisible(bool visible)
	{
		_native.SetIsVisible(visible.AsComBool());
	}
}
