using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Designer;

namespace Avalonia.DesignerSupport.Remote;

internal class FileWatcherTransport : IAvaloniaRemoteTransportConnection, IDisposable, ITransportWithEnforcedMethod
{
	private readonly string _appPath;

	private string _path;

	private string _lastContents;

	private bool _disposed;

	private Action<IAvaloniaRemoteTransportConnection, object> _onMessage;

	public string PreviewerMethod => "html";

	public event Action<IAvaloniaRemoteTransportConnection, object> OnMessage
	{
		add
		{
			_onMessage = (Action<IAvaloniaRemoteTransportConnection, object>)Delegate.Combine(_onMessage, value);
		}
		remove
		{
			_onMessage = (Action<IAvaloniaRemoteTransportConnection, object>)Delegate.Remove(_onMessage, value);
		}
	}

	public event Action<IAvaloniaRemoteTransportConnection, Exception> OnException
	{
		add
		{
		}
		remove
		{
		}
	}

	public FileWatcherTransport(Uri file, string appPath)
	{
		_appPath = appPath;
		_path = file.LocalPath;
	}

	public void Dispose()
	{
		_disposed = true;
	}

	private void Dump(object o, string pad)
	{
		PropertyInfo[] properties = o.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			Console.Write(pad + propertyInfo.Name + ": ");
			object value = propertyInfo.GetValue(o);
			if (value == null || value.GetType().IsPrimitive || value is string || value is Guid)
			{
				Console.WriteLine(value);
				continue;
			}
			Console.WriteLine();
			Dump(value, pad + "    ");
		}
	}

	public Task Send(object data)
	{
		Console.WriteLine(data.GetType().Name);
		Dump(data, "    ");
		return Task.CompletedTask;
	}

	public void Start()
	{
		UpdaterThread();
	}

	private async void UpdaterThread()
	{
		while (!_disposed)
		{
			string text = File.ReadAllText(_path);
			if (text != _lastContents)
			{
				Console.WriteLine("Triggering XAML update");
				_lastContents = text;
				_onMessage?.Invoke(this, new UpdateXamlMessage
				{
					Xaml = text,
					AssemblyPath = _appPath
				});
			}
			await Task.Delay(100);
		}
	}
}
