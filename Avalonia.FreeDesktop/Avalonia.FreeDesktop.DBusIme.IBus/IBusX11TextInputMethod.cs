using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Logging;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop.DBusIme.IBus;

internal class IBusX11TextInputMethod : DBusTextInputMethodBase
{
	private OrgFreedesktopIBusService? _service;

	private OrgFreedesktopIBusInputContext? _context;

	public IBusX11TextInputMethod(Connection connection)
		: base(connection, "org.freedesktop.portal.IBus")
	{
	}

	protected override async Task<bool> Connect(string name)
	{
		ObjectPath objectPath = await new OrgFreedesktopIBusPortal(base.Connection, name, "/org/freedesktop/IBus").CreateInputContextAsync(GetAppName());
		_service = new OrgFreedesktopIBusService(base.Connection, name, objectPath);
		_context = new OrgFreedesktopIBusInputContext(base.Connection, name, objectPath);
		AddDisposable(await _context.WatchCommitTextAsync(OnCommitText));
		AddDisposable(await _context.WatchForwardKeyEventAsync(OnForwardKey));
		Enqueue(() => _context.SetCapabilitiesAsync(8u));
		return true;
	}

	private void OnForwardKey(Exception? e, (uint keyval, uint keycode, uint state) k)
	{
		if (e != null)
		{
			Logger.TryGet(LogEventLevel.Error, "FreeDesktopPlatform")?.Log(this, $"OnForwardKey failed: {e}");
			return;
		}
		IBusModifierMask item = (IBusModifierMask)k.state;
		KeyModifiers keyModifiers = KeyModifiers.None;
		if (item.HasAllFlags(IBusModifierMask.ControlMask))
		{
			keyModifiers |= KeyModifiers.Control;
		}
		if (item.HasAllFlags(IBusModifierMask.Mod1Mask))
		{
			keyModifiers |= KeyModifiers.Alt;
		}
		if (item.HasAllFlags(IBusModifierMask.ShiftMask))
		{
			keyModifiers |= KeyModifiers.Shift;
		}
		if (item.HasAllFlags(IBusModifierMask.Mod4Mask))
		{
			keyModifiers |= KeyModifiers.Meta;
		}
		FireForward(new X11InputMethodForwardedKey
		{
			KeyVal = (int)k.keyval,
			Type = (item.HasAllFlags(IBusModifierMask.ReleaseMask) ? RawKeyEventType.KeyUp : RawKeyEventType.KeyDown),
			Modifiers = keyModifiers
		});
	}

	private void OnCommitText(Exception? e, DBusVariantItem variantItem)
	{
		if (e != null)
		{
			Logger.TryGet(LogEventLevel.Error, "FreeDesktopPlatform")?.Log(this, $"OnCommitText failed: {e}");
		}
		else if (variantItem.Value is DBusStructItem { Count: >=3 } dBusStructItem && dBusStructItem[2] is DBusStringItem dBusStringItem)
		{
			FireCommit(dBusStringItem.Value);
		}
	}

	protected override Task DisconnectAsync()
	{
		return _service?.DestroyAsync() ?? Task.CompletedTask;
	}

	protected override void OnDisconnected()
	{
		_service = null;
		_context = null;
		base.OnDisconnected();
	}

	protected override Task SetCursorRectCore(PixelRect rect)
	{
		return _context?.SetCursorLocationAsync(rect.X, rect.Y, rect.Width, rect.Height) ?? Task.CompletedTask;
	}

	protected override Task SetActiveCore(bool active)
	{
		return ((!active) ? _context?.FocusOutAsync() : _context?.FocusInAsync()) ?? Task.CompletedTask;
	}

	protected override Task ResetContextCore()
	{
		return _context?.ResetAsync() ?? Task.CompletedTask;
	}

	protected override Task<bool> HandleKeyCore(RawKeyEventArgs args, int keyVal, int keyCode)
	{
		IBusModifierMask busModifierMask = (IBusModifierMask)0;
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Control))
		{
			busModifierMask |= IBusModifierMask.ControlMask;
		}
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Alt))
		{
			busModifierMask |= IBusModifierMask.Mod1Mask;
		}
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Shift))
		{
			busModifierMask |= IBusModifierMask.ShiftMask;
		}
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Meta))
		{
			busModifierMask |= IBusModifierMask.Mod4Mask;
		}
		if (args.Type == RawKeyEventType.KeyUp)
		{
			busModifierMask |= IBusModifierMask.ReleaseMask;
		}
		if (_context == null)
		{
			return Task.FromResult(result: false);
		}
		return _context.ProcessKeyEventAsync((uint)keyVal, (uint)keyCode, (uint)busModifierMask);
	}

	public override void SetOptions(TextInputOptions options)
	{
	}
}
