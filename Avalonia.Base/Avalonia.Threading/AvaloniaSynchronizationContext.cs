using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Avalonia.Utilities;

namespace Avalonia.Threading;

public class AvaloniaSynchronizationContext : SynchronizationContext
{
	public record struct RestoreContext(SynchronizationContext? oldContext) : IDisposable
	{
		private readonly SynchronizationContext? _oldContext = oldContext;

		private bool _needRestore = true;

		public void Dispose()
		{
			if (_needRestore)
			{
				SynchronizationContext.SetSynchronizationContext(oldContext);
				_needRestore = false;
			}
		}

		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	internal readonly DispatcherPriority Priority;

	private readonly NonPumpingLockHelper.IHelperImpl? _nonPumpingHelper = AvaloniaLocator.Current.GetService<NonPumpingLockHelper.IHelperImpl>();

	public static bool AutoInstall { get; set; } = true;

	public AvaloniaSynchronizationContext()
		: this(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
	{
	}

	internal AvaloniaSynchronizationContext(bool isStaThread)
	{
		if (_nonPumpingHelper != null && isStaThread)
		{
			SetWaitNotificationRequired();
		}
	}

	public AvaloniaSynchronizationContext(DispatcherPriority priority)
	{
		Priority = priority;
	}

	public static void InstallIfNeeded()
	{
		if (AutoInstall && !(SynchronizationContext.Current is AvaloniaSynchronizationContext))
		{
			SynchronizationContext.SetSynchronizationContext(Dispatcher.UIThread.GetContextWithPriority(DispatcherPriority.Normal));
		}
	}

	public override void Post(SendOrPostCallback d, object? state)
	{
		Dispatcher.UIThread.Post(d, state, Priority);
	}

	public override void Send(SendOrPostCallback d, object? state)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			d(state);
			return;
		}
		Dispatcher.UIThread.InvokeAsync(delegate
		{
			d(state);
		}, DispatcherPriority.Send).GetAwaiter().GetResult();
	}

	public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
	{
		if (_nonPumpingHelper != null && Dispatcher.UIThread.CheckAccess() && Dispatcher.UIThread.DisabledProcessingCount > 0)
		{
			return _nonPumpingHelper.Wait(waitHandles, waitAll, millisecondsTimeout);
		}
		return base.Wait(waitHandles, waitAll, millisecondsTimeout);
	}

	public static RestoreContext Ensure(DispatcherPriority priority)
	{
		return Ensure(Dispatcher.UIThread, priority);
	}

	public static RestoreContext Ensure(Dispatcher dispatcher, DispatcherPriority priority)
	{
		if (SynchronizationContext.Current is AvaloniaSynchronizationContext avaloniaSynchronizationContext && avaloniaSynchronizationContext.Priority == priority)
		{
			return default(RestoreContext);
		}
		SynchronizationContext? current = SynchronizationContext.Current;
		dispatcher.VerifyAccess();
		SynchronizationContext.SetSynchronizationContext(dispatcher.GetContextWithPriority(priority));
		return new RestoreContext(current);
	}
}
