using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Logging;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop.DBusIme.Fcitx;

internal class FcitxX11TextInputMethod : DBusTextInputMethodBase
{
	private FcitxICWrapper? _context;

	private FcitxCapabilityFlags? _lastReportedFlags;

	public FcitxX11TextInputMethod(Connection connection)
		: base(connection, "org.fcitx.Fcitx", "org.freedesktop.portal.Fcitx")
	{
	}

	protected override async Task<bool> Connect(string name)
	{
		if (name == "org.fcitx.Fcitx")
		{
			(int, bool, uint, uint, uint, uint) tuple = await new OrgFcitxFcitxInputMethod(base.Connection, name, "/inputmethod").CreateICv3Async(GetAppName(), Process.GetCurrentProcess().Id);
			OrgFcitxFcitxInputContext old = new OrgFcitxFcitxInputContext(base.Connection, name, $"/inputcontext_{tuple.Item1}");
			_context = new FcitxICWrapper(old);
		}
		else
		{
			(ObjectPath, byte[]) tuple2 = await new OrgFcitxFcitxInputMethod1(base.Connection, name, "/inputmethod").CreateInputContextAsync(new(string, string)[1] { ("appName", GetAppName()) });
			OrgFcitxFcitxInputContext1 modern = new OrgFcitxFcitxInputContext1(base.Connection, name, tuple2.Item1);
			_context = new FcitxICWrapper(modern);
		}
		AddDisposable(await _context.WatchCommitStringAsync(OnCommitString));
		AddDisposable(await _context.WatchForwardKeyAsync(OnForward));
		return true;
	}

	protected override Task DisconnectAsync()
	{
		return _context?.DestroyICAsync() ?? Task.CompletedTask;
	}

	protected override void OnDisconnected()
	{
		_context = null;
	}

	protected override void Reset()
	{
		_lastReportedFlags = null;
		base.Reset();
	}

	protected override Task SetCursorRectCore(PixelRect cursorRect)
	{
		return _context?.SetCursorRectAsync(cursorRect.X, cursorRect.Y, Math.Max(1, cursorRect.Width), Math.Max(1, cursorRect.Height)) ?? Task.CompletedTask;
	}

	protected override Task SetActiveCore(bool active)
	{
		return ((!active) ? _context?.FocusOutAsync() : _context?.FocusInAsync()) ?? Task.CompletedTask;
	}

	protected override Task ResetContextCore()
	{
		return _context?.ResetAsync() ?? Task.CompletedTask;
	}

	protected override async Task<bool> HandleKeyCore(RawKeyEventArgs args, int keyVal, int keyCode)
	{
		FcitxKeyState fcitxKeyState = FcitxKeyState.FcitxKeyState_None;
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Control))
		{
			fcitxKeyState |= FcitxKeyState.FcitxKeyState_Ctrl;
		}
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Alt))
		{
			fcitxKeyState |= FcitxKeyState.FcitxKeyState_Alt;
		}
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Shift))
		{
			fcitxKeyState |= FcitxKeyState.FcitxKeyState_Shift;
		}
		if (args.Modifiers.HasAllFlags(RawInputModifiers.Meta))
		{
			fcitxKeyState |= FcitxKeyState.FcitxKeyState_Super;
		}
		FcitxKeyEventType type = ((args.Type != 0) ? FcitxKeyEventType.FCITX_RELEASE_KEY : FcitxKeyEventType.FCITX_PRESS_KEY);
		if (_context != null)
		{
			return await _context.ProcessKeyEventAsync((uint)keyVal, (uint)keyCode, (uint)fcitxKeyState, (int)type, (uint)args.Timestamp).ConfigureAwait(continueOnCapturedContext: false);
		}
		return false;
	}

	public override void SetOptions(TextInputOptions options)
	{
		Enqueue(async delegate
		{
			if (_context != null)
			{
				FcitxCapabilityFlags fcitxCapabilityFlags = FcitxCapabilityFlags.CAPACITY_NONE;
				if (options.Lowercase)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_LOWERCASE;
				}
				if (options.Uppercase)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_UPPERCASE;
				}
				if (!options.AutoCapitalization)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_NOAUTOUPPERCASE;
				}
				if (options.ContentType == TextInputContentType.Email)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_EMAIL;
				}
				else if (options.ContentType == TextInputContentType.Number)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_NUMBER;
				}
				else if (options.ContentType == TextInputContentType.Password)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_PASSWORD;
				}
				else if (options.ContentType == TextInputContentType.Digits)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_DIALABLE;
				}
				else if (options.ContentType == TextInputContentType.Url)
				{
					fcitxCapabilityFlags |= FcitxCapabilityFlags.CAPACITY_URL;
				}
				if (fcitxCapabilityFlags != _lastReportedFlags)
				{
					_lastReportedFlags = fcitxCapabilityFlags;
					await _context.SetCapacityAsync((uint)fcitxCapabilityFlags);
				}
			}
		});
	}

	private void OnForward(Exception? e, (uint keyval, uint state, int type) ev)
	{
		uint item = ev.state;
		KeyModifiers keyModifiers = KeyModifiers.None;
		if (((FcitxKeyState)item).HasAllFlags(FcitxKeyState.FcitxKeyState_Ctrl))
		{
			keyModifiers |= KeyModifiers.Control;
		}
		if (((FcitxKeyState)item).HasAllFlags(FcitxKeyState.FcitxKeyState_Alt))
		{
			keyModifiers |= KeyModifiers.Alt;
		}
		if (((FcitxKeyState)item).HasAllFlags(FcitxKeyState.FcitxKeyState_Shift))
		{
			keyModifiers |= KeyModifiers.Shift;
		}
		if (((FcitxKeyState)item).HasAllFlags(FcitxKeyState.FcitxKeyState_Super))
		{
			keyModifiers |= KeyModifiers.Meta;
		}
		FireForward(new X11InputMethodForwardedKey
		{
			Modifiers = keyModifiers,
			KeyVal = (int)ev.keyval,
			Type = ((ev.type != 0) ? RawKeyEventType.KeyUp : RawKeyEventType.KeyDown)
		});
	}

	private void OnCommitString(Exception? e, string s)
	{
		if (e != null)
		{
			Logger.TryGet(LogEventLevel.Error, "FreeDesktopPlatform")?.Log(this, $"OnCommitString failed: {e}");
		}
		else
		{
			FireCommit(s);
		}
	}
}
