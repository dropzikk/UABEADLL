using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Avalonia.Controls;
using Avalonia.DesignerSupport.Remote.HtmlTransport;
using Avalonia.Remote.Protocol;
using Avalonia.Remote.Protocol.Designer;
using Avalonia.Remote.Protocol.Viewport;
using Avalonia.Threading;

namespace Avalonia.DesignerSupport.Remote;

public class RemoteDesignerEntryPoint
{
	private class CommandLineArgs
	{
		public string AppPath { get; set; }

		public Uri Transport { get; set; }

		public Uri HtmlMethodListenUri { get; set; }

		public string Method { get; set; } = "avalonia-remote";

		public string SessionId { get; set; } = Guid.NewGuid().ToString();
	}

	internal static class Methods
	{
		public const string AvaloniaRemote = "avalonia-remote";

		public const string Win32 = "win32";

		public const string Html = "html";
	}

	private interface IAppInitializer
	{
		IAvaloniaRemoteTransportConnection ConfigureApp(IAvaloniaRemoteTransportConnection transport, CommandLineArgs args, object obj);
	}

	private class AppInitializer : IAppInitializer
	{
		public IAvaloniaRemoteTransportConnection ConfigureApp(IAvaloniaRemoteTransportConnection transport, CommandLineArgs args, object obj)
		{
			AppBuilder appBuilder = (AppBuilder)obj;
			appBuilder = appBuilder.UseStandardRuntimePlatformSubsystem();
			if (args.Method == "avalonia-remote")
			{
				appBuilder.UseWindowingSubsystem(delegate
				{
					PreviewerWindowingPlatform.Initialize(transport);
				});
			}
			if (args.Method == "html")
			{
				transport = new HtmlWebSocketTransport(transport, args.HtmlMethodListenUri ?? new Uri("http://localhost:5000"));
				appBuilder.UseWindowingSubsystem(delegate
				{
					PreviewerWindowingPlatform.Initialize(transport);
				});
			}
			if (args.Method == "win32")
			{
				appBuilder.UseWindowingSubsystem(GetInitializer("Avalonia.Win32"), "Win32");
			}
			appBuilder.SetupWithoutStarting();
			return transport;
		}

		private static Action GetInitializer(string assemblyName)
		{
			return delegate
			{
				Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
				string name = string.Concat(str2: assemblyName.Replace("Avalonia.", string.Empty) + "Platform", str0: assemblyName, str1: ".");
				assembly.GetType(name).GetRuntimeMethod("Initialize", Type.EmptyTypes).Invoke(null, null);
			};
		}
	}

	private static ClientSupportedPixelFormatsMessage s_supportedPixelFormats;

	private static ClientViewportAllocatedMessage s_viewportAllocatedMessage;

	private static ClientRenderInfoMessage s_renderInfoMessage;

	private static double s_lastRenderScaling = 1.0;

	private static IAvaloniaRemoteTransportConnection s_transport;

	private const string BuilderMethodName = "BuildAvaloniaApp";

	private static Window s_currentWindow;

	private static Exception Die(string error)
	{
		if (error != null)
		{
			Console.Error.WriteLine(error);
			Console.Error.Flush();
		}
		Environment.Exit(1);
		return new Exception("APPEXIT");
	}

	private static void Log(string message)
	{
		Console.WriteLine(message);
	}

	private static Exception PrintUsage()
	{
		Console.Error.WriteLine("Usage: --transport transport_spec --session-id sid --method method app");
		Console.Error.WriteLine();
		Console.Error.WriteLine("--transport: transport used for communication with the IDE");
		Console.Error.WriteLine("    'tcp-bson' (e. g. 'tcp-bson://127.0.0.1:30243/') - TCP-based transport with BSON serialization of messages defined in Avalonia.Remote.Protocol");
		Console.Error.WriteLine("    'file' (e. g. 'file://C://my/file.xaml' - pseudo-transport that triggers XAML updates on file changes, useful as a standalone previewer tool, always uses http preview method");
		Console.Error.WriteLine();
		Console.Error.WriteLine("--session-id: session id to be sent to IDE process");
		Console.Error.WriteLine();
		Console.Error.WriteLine("--method: the way the XAML is displayed");
		Console.Error.WriteLine("    'avalonia-remote' - binary image is sent via transport connection in FrameMessage");
		Console.Error.WriteLine("    'win32' - XAML is displayed in win32 window (handle could be obtained from UpdateXamlResultMessage), IDE is responsible to use user32!SetParent");
		Console.Error.WriteLine("    'html' - Previewer starts an HTML server and displays XAML previewer as a web page");
		Console.Error.WriteLine();
		Console.Error.WriteLine("--html-url - endpoint for HTML method to listen on, e. g. http://127.0.0.1:8081");
		Console.Error.WriteLine();
		Console.Error.WriteLine("Example: --transport tcp-bson://127.0.0.1:30243/ --session-id 123 --method avalonia-remote MyApp.exe");
		Console.Error.Flush();
		return Die(null);
	}

	private static CommandLineArgs ParseCommandLineArgs(string[] args)
	{
		CommandLineArgs rv = new CommandLineArgs();
		Action<string> action = null;
		try
		{
			foreach (string text in args)
			{
				if (action != null)
				{
					action(text);
					action = null;
					continue;
				}
				switch (text)
				{
				case "--transport":
					action = delegate(string a)
					{
						rv.Transport = new Uri(a, UriKind.Absolute);
					};
					continue;
				case "--method":
					action = delegate(string a)
					{
						rv.Method = a;
					};
					continue;
				case "--html-url":
					action = delegate(string a)
					{
						rv.HtmlMethodListenUri = new Uri(a, UriKind.Absolute);
					};
					continue;
				case "--session-id":
					action = delegate(string a)
					{
						rv.SessionId = a;
					};
					continue;
				}
				if (rv.AppPath == null)
				{
					rv.AppPath = text;
				}
				else
				{
					PrintUsage();
				}
			}
			if (rv.AppPath == null || rv.Transport == null)
			{
				PrintUsage();
			}
		}
		catch
		{
			PrintUsage();
		}
		if (action != null)
		{
			PrintUsage();
		}
		return rv;
	}

	private static IAvaloniaRemoteTransportConnection CreateTransport(CommandLineArgs args)
	{
		Uri transport = args.Transport;
		if (transport.Scheme == "tcp-bson")
		{
			return new BsonTcpTransport().Connect(IPAddress.Parse(transport.Host), transport.Port).Result;
		}
		if (transport.Scheme == "file")
		{
			return new FileWatcherTransport(transport, args.AppPath);
		}
		PrintUsage();
		return null;
	}

	public static void Main(string[] cmdline)
	{
		CommandLineArgs commandLineArgs = ParseCommandLineArgs(cmdline);
		IAvaloniaRemoteTransportConnection avaloniaRemoteTransportConnection = CreateTransport(commandLineArgs);
		if (avaloniaRemoteTransportConnection is ITransportWithEnforcedMethod transportWithEnforcedMethod)
		{
			commandLineArgs.Method = transportWithEnforcedMethod.PreviewerMethod;
		}
		MethodInfo entryPoint = Assembly.LoadFile(Path.GetFullPath(commandLineArgs.AppPath)).EntryPoint;
		if (entryPoint == null)
		{
			throw Die("Assembly " + commandLineArgs.AppPath + " doesn't have an entry point");
		}
		Log("Obtaining AppBuilder instance from " + entryPoint.DeclaringType.FullName);
		AppBuilder obj = AppBuilder.Configure(entryPoint.DeclaringType);
		Design.IsDesignMode = true;
		Log("Initializing application in design mode");
		avaloniaRemoteTransportConnection = (s_transport = ((IAppInitializer)Activator.CreateInstance(typeof(AppInitializer))).ConfigureApp(avaloniaRemoteTransportConnection, commandLineArgs, obj));
		avaloniaRemoteTransportConnection.OnMessage += OnTransportMessage;
		avaloniaRemoteTransportConnection.OnException += delegate(IAvaloniaRemoteTransportConnection t, Exception e)
		{
			Die(e.ToString());
		};
		avaloniaRemoteTransportConnection.Start();
		Log("Sending StartDesignerSessionMessage");
		avaloniaRemoteTransportConnection.Send(new StartDesignerSessionMessage
		{
			SessionId = commandLineArgs.SessionId
		});
		Dispatcher.UIThread.MainLoop(CancellationToken.None);
	}

	private static void RebuildPreFlight()
	{
		PreviewerWindowingPlatform.PreFlightMessages = new List<object> { s_supportedPixelFormats, s_viewportAllocatedMessage, s_renderInfoMessage };
	}

	private static void OnTransportMessage(IAvaloniaRemoteTransportConnection transport, object obj)
	{
		Dispatcher.UIThread.Post(delegate
		{
			if (obj is ClientSupportedPixelFormatsMessage clientSupportedPixelFormatsMessage)
			{
				s_supportedPixelFormats = clientSupportedPixelFormatsMessage;
				RebuildPreFlight();
			}
			if (obj is ClientRenderInfoMessage clientRenderInfoMessage)
			{
				s_renderInfoMessage = clientRenderInfoMessage;
				RebuildPreFlight();
			}
			if (obj is ClientViewportAllocatedMessage clientViewportAllocatedMessage)
			{
				s_viewportAllocatedMessage = clientViewportAllocatedMessage;
				RebuildPreFlight();
			}
			if (obj is UpdateXamlMessage updateXamlMessage)
			{
				if (s_currentWindow != null)
				{
					s_lastRenderScaling = s_currentWindow.RenderScaling;
				}
				try
				{
					s_currentWindow?.Close();
				}
				catch
				{
				}
				s_currentWindow = null;
				try
				{
					s_currentWindow = DesignWindowLoader.LoadDesignerWindow(updateXamlMessage.Xaml, updateXamlMessage.AssemblyPath, updateXamlMessage.XamlFileProjectPath, s_lastRenderScaling);
					s_transport.Send(new UpdateXamlResultMessage
					{
						Handle = s_currentWindow.PlatformImpl?.Handle?.Handle.ToString()
					});
				}
				catch (Exception ex)
				{
					s_transport.Send(new UpdateXamlResultMessage
					{
						Error = ex.ToString(),
						Exception = new ExceptionDetails(ex)
					});
				}
			}
		});
	}
}
