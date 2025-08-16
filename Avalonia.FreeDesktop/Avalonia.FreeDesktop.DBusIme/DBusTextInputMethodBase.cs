using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Logging;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop.DBusIme;

internal abstract class DBusTextInputMethodBase : IX11InputMethodControl, IDisposable, ITextInputMethodImpl
{
	private List<IDisposable> _disposables = new List<IDisposable>();

	private Queue<string> _onlineNamesQueue = new Queue<string>();

	private readonly string[] _knownNames;

	private bool _connecting;

	private string? _currentName;

	private DBusCallQueue _queue;

	private bool _windowActive;

	private bool? _imeActive;

	private Rect _logicalRect;

	private PixelRect? _lastReportedRect;

	private double _scaling = 1.0;

	private PixelPoint _windowPosition;

	private TextInputMethodClient? _client;

	private Action<string>? _onCommit;

	private Action<X11InputMethodForwardedKey>? _onForward;

	protected Connection Connection { get; }

	protected bool IsConnected => _currentName != null;

	public TextInputMethodClient Client => _client;

	public bool IsActive => _client != null;

	bool IX11InputMethodControl.IsEnabled
	{
		get
		{
			if (IsConnected)
			{
				return _imeActive == true;
			}
			return false;
		}
	}

	event Action<string> IX11InputMethodControl.Commit
	{
		add
		{
			_onCommit = (Action<string>)Delegate.Combine(_onCommit, value);
		}
		remove
		{
			_onCommit = (Action<string>)Delegate.Remove(_onCommit, value);
		}
	}

	event Action<X11InputMethodForwardedKey> IX11InputMethodControl.ForwardKey
	{
		add
		{
			_onForward = (Action<X11InputMethodForwardedKey>)Delegate.Combine(_onForward, value);
		}
		remove
		{
			_onForward = (Action<X11InputMethodForwardedKey>)Delegate.Remove(_onForward, value);
		}
	}

	public DBusTextInputMethodBase(Connection connection, params string[] knownNames)
	{
		_queue = new DBusCallQueue(QueueOnErrorAsync);
		Connection = connection;
		_knownNames = knownNames;
		WatchAsync();
	}

	private async Task WatchAsync()
	{
		string[] knownNames = _knownNames;
		foreach (string name in knownNames)
		{
			OrgFreedesktopDBus dbus = new OrgFreedesktopDBus(Connection, "org.freedesktop.DBus", "/org/freedesktop/DBus");
			List<IDisposable> disposables = _disposables;
			disposables.Add(await dbus.WatchNameOwnerChangedAsync(OnNameChange));
			OnNameChange(null, (ServiceName: name, OldOwner: null, NewOwner: await dbus.GetNameOwnerAsync(name)));
		}
	}

	protected abstract Task<bool> Connect(string name);

	protected string GetAppName()
	{
		return Application.Current?.Name ?? Assembly.GetEntryAssembly()?.GetName()?.Name ?? "Avalonia";
	}

	private async void OnNameChange(Exception? e, (string ServiceName, string? OldOwner, string? NewOwner) args)
	{
		if (e != null)
		{
			Logger.TryGet(LogEventLevel.Error, "FreeDesktopPlatform")?.Log(this, $"OnNameChange failed: {e}");
			return;
		}
		if (args.NewOwner != null && _currentName == null)
		{
			_onlineNamesQueue.Enqueue(args.ServiceName);
			if (!_connecting)
			{
				_connecting = true;
				try
				{
					while (_onlineNamesQueue.Count > 0)
					{
						string name = _onlineNamesQueue.Dequeue();
						try
						{
							if (await Connect(name))
							{
								_onlineNamesQueue.Clear();
								_currentName = name;
								return;
							}
						}
						catch (Exception ex)
						{
							Logger.TryGet(LogEventLevel.Error, "IME")?.Log(this, "Unable to create IME input context:\n" + ex);
						}
					}
				}
				finally
				{
					_connecting = false;
				}
			}
		}
		if (args.NewOwner != null || !(args.ServiceName == _currentName))
		{
			return;
		}
		_currentName = null;
		foreach (IDisposable disposable in _disposables)
		{
			disposable.Dispose();
		}
		_disposables.Clear();
		OnDisconnected();
		Reset();
		WatchAsync();
	}

	protected virtual Task DisconnectAsync()
	{
		return Task.CompletedTask;
	}

	protected virtual void OnDisconnected()
	{
	}

	protected virtual void Reset()
	{
		_lastReportedRect = null;
		_imeActive = null;
	}

	private async Task QueueOnErrorAsync(Exception e)
	{
		Logger.TryGet(LogEventLevel.Error, "IME")?.Log(this, "Error:\n" + e);
		try
		{
			await DisconnectAsync();
		}
		catch (Exception ex)
		{
			Logger.TryGet(LogEventLevel.Error, "IME")?.Log(this, "Error while destroying the context:\n" + ex);
		}
		OnDisconnected();
		_currentName = null;
	}

	protected void Enqueue(Func<Task> cb)
	{
		_queue.Enqueue(cb);
	}

	protected void AddDisposable(IDisposable? d)
	{
		if (d != null)
		{
			_disposables.Add(d);
		}
	}

	public void Dispose()
	{
		foreach (IDisposable disposable in _disposables)
		{
			disposable.Dispose();
		}
		_disposables.Clear();
		DisconnectAsync();
		_currentName = null;
	}

	protected abstract Task SetCursorRectCore(PixelRect rect);

	protected abstract Task SetActiveCore(bool active);

	protected abstract Task ResetContextCore();

	protected abstract Task<bool> HandleKeyCore(RawKeyEventArgs args, int keyVal, int keyCode);

	private void UpdateActive()
	{
		_queue.Enqueue(async delegate
		{
			if (IsConnected)
			{
				bool flag = _windowActive && IsActive;
				if (flag != _imeActive)
				{
					_imeActive = flag;
					await SetActiveCore(flag);
				}
			}
		});
	}

	void IX11InputMethodControl.SetWindowActive(bool active)
	{
		_windowActive = active;
		UpdateActive();
	}

	void ITextInputMethodImpl.SetClient(TextInputMethodClient? client)
	{
		_client = client;
		UpdateActive();
	}

	async ValueTask<bool> IX11InputMethodControl.HandleEventAsync(RawKeyEventArgs args, int keyVal, int keyCode)
	{
		bool result = default(bool);
		object obj;
		int num;
		try
		{
			result = await _queue.EnqueueAsync(async () => await HandleKeyCore(args, keyVal, keyCode));
			return result;
		}
		catch (OperationCanceledException)
		{
			return false;
		}
		catch (Exception ex2)
		{
			obj = ex2;
			num = 1;
		}
		if (num != 1)
		{
			return result;
		}
		Exception e = (Exception)obj;
		await QueueOnErrorAsync(e);
		return false;
	}

	protected void FireCommit(string s)
	{
		_onCommit?.Invoke(s);
	}

	protected void FireForward(X11InputMethodForwardedKey k)
	{
		_onForward?.Invoke(k);
	}

	private void UpdateCursorRect()
	{
		_queue.Enqueue(async delegate
		{
			if (IsConnected)
			{
				PixelRect pixelRect = PixelRect.FromRect(_logicalRect, _scaling).Translate(_windowPosition);
				PixelRect value = pixelRect;
				PixelRect? lastReportedRect = _lastReportedRect;
				if (value != lastReportedRect)
				{
					_lastReportedRect = pixelRect;
					await SetCursorRectCore(pixelRect);
				}
			}
		});
	}

	void IX11InputMethodControl.UpdateWindowInfo(PixelPoint position, double scaling)
	{
		_windowPosition = position;
		_scaling = scaling;
		UpdateCursorRect();
	}

	void ITextInputMethodImpl.SetCursorRect(Rect rect)
	{
		_logicalRect = rect;
		UpdateCursorRect();
	}

	public abstract void SetOptions(TextInputOptions options);

	void ITextInputMethodImpl.Reset()
	{
		Reset();
		_queue.Enqueue(async delegate
		{
			if (IsConnected)
			{
				await ResetContextCore();
			}
		});
	}
}
