using System;
using Avalonia.Controls.Platform;
using Avalonia.Logging;
using Avalonia.Platform;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop;

internal class DBusTrayIconImpl : ITrayIconImpl, IDisposable
{
	private static int s_trayIconInstanceId;

	public static readonly (int, int, byte[]) EmptyPixmap = (1, 1, new byte[4] { 255, 0, 0, 0 });

	private readonly ObjectPath _dbusMenuPath;

	private readonly Connection? _connection;

	private readonly OrgFreedesktopDBus? _dBus;

	private IDisposable? _serviceWatchDisposable;

	private StatusNotifierItemDbusObj? _statusNotifierItemDbusObj;

	private OrgKdeStatusNotifierWatcher? _statusNotifierWatcher;

	private (int, int, byte[]) _icon;

	private string? _sysTrayServiceName;

	private string? _tooltipText;

	private bool _isDisposed;

	private bool _serviceConnected;

	private bool _isVisible = true;

	public bool IsActive { get; private set; }

	public INativeMenuExporter? MenuExporter { get; }

	public Action? OnClicked { get; set; }

	public Func<IWindowIconImpl?, uint[]>? IconConverterDelegate { get; set; }

	public DBusTrayIconImpl()
	{
		_connection = DBusHelper.TryCreateNewConnection();
		if (_connection == null)
		{
			Logger.TryGet(LogEventLevel.Error, "DBUS")?.Log(this, "Unable to get a dbus connection for system tray icons.");
			return;
		}
		IsActive = true;
		_dBus = new OrgFreedesktopDBus(_connection, "org.freedesktop.DBus", "/org/freedesktop/DBus");
		_dbusMenuPath = DBusMenuExporter.GenerateDBusMenuObjPath;
		MenuExporter = DBusMenuExporter.TryCreateDetachedNativeMenu(_dbusMenuPath, _connection);
		WatchAsync();
	}

	private async void WatchAsync()
	{
		_ = 1;
		try
		{
			_serviceWatchDisposable = await _dBus.WatchNameOwnerChangedAsync(delegate(Exception? _, (string Item1, string Item2, string Item3) x)
			{
				OnNameChange(x.Item1, x.Item3);
			});
			OnNameChange("org.kde.StatusNotifierWatcher", await _dBus.GetNameOwnerAsync("org.kde.StatusNotifierWatcher"));
		}
		catch
		{
			_serviceWatchDisposable = null;
			Logger.TryGet(LogEventLevel.Error, "DBUS")?.Log(this, "Interface 'org.kde.StatusNotifierWatcher' is unavailable.");
		}
	}

	private void OnNameChange(string name, string? newOwner)
	{
		if (_isDisposed || _connection == null || name != "org.kde.StatusNotifierWatcher")
		{
			return;
		}
		if (!_serviceConnected && newOwner != null)
		{
			_serviceConnected = true;
			_statusNotifierWatcher = new OrgKdeStatusNotifierWatcher(_connection, "org.kde.StatusNotifierWatcher", "/StatusNotifierWatcher");
			DestroyTrayIcon();
			if (_isVisible)
			{
				CreateTrayIcon();
			}
		}
		else if (_serviceConnected && newOwner == null)
		{
			DestroyTrayIcon();
			_serviceConnected = false;
		}
	}

	private async void CreateTrayIcon()
	{
		if (_connection != null && _serviceConnected && !_isDisposed && _statusNotifierWatcher != null)
		{
			int processId = Environment.ProcessId;
			int num = s_trayIconInstanceId++;
			_sysTrayServiceName = FormattableString.Invariant($"org.kde.StatusNotifierItem-{processId}-{num}");
			_statusNotifierItemDbusObj = new StatusNotifierItemDbusObj(_connection, _dbusMenuPath);
			_connection.AddMethodHandler(_statusNotifierItemDbusObj);
			await _dBus.RequestNameAsync(_sysTrayServiceName, 0u);
			await _statusNotifierWatcher.RegisterStatusNotifierItemAsync(_sysTrayServiceName);
			_statusNotifierItemDbusObj.SetTitleAndTooltip(_tooltipText);
			_statusNotifierItemDbusObj.SetIcon(_icon);
			_statusNotifierItemDbusObj.ActivationDelegate += OnClicked;
		}
	}

	private void DestroyTrayIcon()
	{
		if (_connection != null && _serviceConnected && !_isDisposed && _statusNotifierItemDbusObj != null && _sysTrayServiceName != null)
		{
			_dBus.ReleaseNameAsync(_sysTrayServiceName);
		}
	}

	public void Dispose()
	{
		IsActive = false;
		_isDisposed = true;
		DestroyTrayIcon();
		_serviceWatchDisposable?.Dispose();
	}

	public void SetIcon(IWindowIconImpl? icon)
	{
		if (_isDisposed || IconConverterDelegate == null)
		{
			return;
		}
		if (icon == null)
		{
			_statusNotifierItemDbusObj?.SetIcon(EmptyPixmap);
			return;
		}
		uint[] array = IconConverterDelegate(icon);
		if (array.Length != 0)
		{
			int num = (int)array[0];
			int num2 = (int)array[1];
			int num3 = num * num2;
			int num4 = 0;
			byte[] array2 = new byte[num * num2 * 4];
			for (int i = 0; i < num3; i++)
			{
				uint num5 = array[i + 2];
				array2[num4++] = (byte)((num5 & 0xFF000000u) >> 24);
				array2[num4++] = (byte)((num5 & 0xFF0000) >> 16);
				array2[num4++] = (byte)((num5 & 0xFF00) >> 8);
				array2[num4++] = (byte)(num5 & 0xFF);
			}
			_icon = (num, num2, array2);
			_statusNotifierItemDbusObj?.SetIcon(_icon);
		}
	}

	public void SetIsVisible(bool visible)
	{
		if (_isDisposed || !_serviceConnected)
		{
			_isVisible = visible;
			return;
		}
		if (visible)
		{
			if (!_isVisible)
			{
				DestroyTrayIcon();
				CreateTrayIcon();
			}
		}
		else if (_isVisible)
		{
			DestroyTrayIcon();
		}
		_isVisible = visible;
	}

	public void SetToolTipText(string? text)
	{
		if (!_isDisposed && text != null)
		{
			_tooltipText = text;
			_statusNotifierItemDbusObj?.SetTitleAndTooltip(_tooltipText);
		}
	}
}
