using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Logging;
using Avalonia.Rendering;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal class DxgiConnection : IRenderTimer
{
	public const uint ENUM_CURRENT_SETTINGS = uint.MaxValue;

	private readonly object _syncLock;

	private IDXGIOutput? _output;

	private Stopwatch? _stopwatch;

	private const string LogArea = "DXGI";

	public bool RunsInBackground => true;

	public event Action<TimeSpan>? Tick;

	public DxgiConnection(object syncLock)
	{
		_syncLock = syncLock;
	}

	public static bool TryCreateAndRegister()
	{
		try
		{
			TryCreateAndRegisterCore();
			return true;
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "DXGI")?.Log(null, "Unable to establish Dxgi: {0}", propertyValue);
			return false;
		}
	}

	private void RunLoop()
	{
		_stopwatch = Stopwatch.StartNew();
		try
		{
			GetBestOutputToVWaitOn();
		}
		catch (Exception ex)
		{
			Logger.TryGet(LogEventLevel.Error, "DXGI")?.Log(this, $"Failed to wait for vblank, Exception: {ex.Message}, HRESULT = {ex.HResult}");
		}
		while (true)
		{
			try
			{
				lock (_syncLock)
				{
					if (_output != null)
					{
						try
						{
							_output.WaitForVBlank();
						}
						catch (Exception ex2)
						{
							Logger.TryGet(LogEventLevel.Error, "DXGI")?.Log(this, $"Failed to wait for vblank, Exception: {ex2.Message}, HRESULT = {ex2.HResult}");
							_output.Dispose();
							_output = null;
							GetBestOutputToVWaitOn();
						}
					}
					else
					{
						UnmanagedMethods.DwmFlush();
					}
					this.Tick?.Invoke(_stopwatch.Elapsed);
				}
			}
			catch (Exception ex3)
			{
				Logger.TryGet(LogEventLevel.Error, "DXGI")?.Log(this, $"Failed to wait for vblank, Exception: {ex3.Message}, HRESULT = {ex3.HResult}");
			}
		}
	}

	private unsafe void GetBestOutputToVWaitOn()
	{
		double num = 0.0;
		Guid riid = MicroComRuntime.GetGuidFor(typeof(IDXGIFactory));
		DirectXUnmanagedMethods.CreateDXGIFactory(ref riid, out var ppFactory);
		using IDXGIFactory iDXGIFactory = MicroComRuntime.CreateProxyFor<IDXGIFactory>(ppFactory, ownsHandle: true);
		void* pObject = null;
		ushort num2 = 0;
		while (iDXGIFactory.EnumAdapters(num2, &pObject) == 0)
		{
			using IDXGIAdapter iDXGIAdapter = MicroComRuntime.CreateProxyFor<IDXGIAdapter>(pObject, ownsHandle: true);
			void* pObject2 = null;
			ushort num3 = 0;
			while (iDXGIAdapter.EnumOutputs(num3, &pObject2) == 0)
			{
				using IDXGIOutput iDXGIOutput = MicroComRuntime.CreateProxyFor<IDXGIOutput>(pObject2, ownsHandle: true);
				DXGI_OUTPUT_DESC desc = iDXGIOutput.Desc;
				HANDLE monitor = desc.Monitor;
				MONITORINFOEXW mONITORINFOEXW = new MONITORINFOEXW
				{
					Base = 
					{
						cbSize = sizeof(MONITORINFOEXW)
					}
				};
				DirectXUnmanagedMethods.GetMonitorInfoW(monitor, (IntPtr)(&mONITORINFOEXW));
				DEVMODEW dEVMODEW = default(DEVMODEW);
				DirectXUnmanagedMethods.EnumDisplaySettingsW(desc.DeviceName, uint.MaxValue, &dEVMODEW);
				if (num < (double)dEVMODEW.dmDisplayFrequency)
				{
					if (_output != null)
					{
						_output.Dispose();
						_output = null;
					}
					_output = iDXGIOutput.CloneReference();
					num = dEVMODEW.dmDisplayFrequency;
				}
				num3++;
			}
			num2++;
		}
	}

	private static bool TryCreateAndRegisterCore()
	{
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		object pumpLock = new object();
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				DxgiConnection dxgiConnection = new DxgiConnection(pumpLock);
				AvaloniaLocator.CurrentMutable.BindToSelf(dxgiConnection);
				AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(dxgiConnection);
				tcs.SetResult(result: true);
				dxgiConnection.RunLoop();
			}
			catch (Exception exception)
			{
				tcs.SetException(exception);
			}
		});
		thread.IsBackground = true;
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		return tcs.Task.Result;
	}
}
