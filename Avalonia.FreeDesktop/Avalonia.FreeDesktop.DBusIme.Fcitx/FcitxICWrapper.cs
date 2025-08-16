using System;
using System.Threading.Tasks;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop.DBusIme.Fcitx;

internal class FcitxICWrapper
{
	private readonly OrgFcitxFcitxInputContext1? _modern;

	private readonly OrgFcitxFcitxInputContext? _old;

	public FcitxICWrapper(OrgFcitxFcitxInputContext old)
	{
		_old = old;
	}

	public FcitxICWrapper(OrgFcitxFcitxInputContext1 modern)
	{
		_modern = modern;
	}

	public Task FocusInAsync()
	{
		return _old?.FocusInAsync() ?? _modern?.FocusInAsync() ?? Task.CompletedTask;
	}

	public Task FocusOutAsync()
	{
		return _old?.FocusOutAsync() ?? _modern?.FocusOutAsync() ?? Task.CompletedTask;
	}

	public Task ResetAsync()
	{
		return _old?.ResetAsync() ?? _modern?.ResetAsync() ?? Task.CompletedTask;
	}

	public Task SetCursorRectAsync(int x, int y, int w, int h)
	{
		return _old?.SetCursorRectAsync(x, y, w, h) ?? _modern?.SetCursorRectAsync(x, y, w, h) ?? Task.CompletedTask;
	}

	public Task DestroyICAsync()
	{
		return _old?.DestroyICAsync() ?? _modern?.DestroyICAsync() ?? Task.CompletedTask;
	}

	public async Task<bool> ProcessKeyEventAsync(uint keyVal, uint keyCode, uint state, int type, uint time)
	{
		if (_old != null)
		{
			return await _old.ProcessKeyEventAsync(keyVal, keyCode, state, type, time) != 0;
		}
		return await (_modern?.ProcessKeyEventAsync(keyVal, keyCode, state, type > 0, time) ?? Task.FromResult(result: false));
	}

	public ValueTask<IDisposable?> WatchCommitStringAsync(Action<Exception?, string> handler)
	{
		return _old?.WatchCommitStringAsync(handler) ?? _modern?.WatchCommitStringAsync(handler) ?? new ValueTask<IDisposable>((IDisposable)null);
	}

	public ValueTask<IDisposable?> WatchForwardKeyAsync(Action<Exception?, (uint keyval, uint state, int type)> handler)
	{
		return _old?.WatchForwardKeyAsync(handler) ?? _modern?.WatchForwardKeyAsync(delegate(Exception? e, (uint keyval, uint state, bool type) ev)
		{
			handler(e, (ev.keyval, ev.state, ev.type ? 1 : 0));
		}) ?? new ValueTask<IDisposable>((IDisposable)null);
	}

	public Task SetCapacityAsync(uint flags)
	{
		return _old?.SetCapacityAsync(flags) ?? _modern?.SetCapabilityAsync(flags) ?? Task.CompletedTask;
	}
}
