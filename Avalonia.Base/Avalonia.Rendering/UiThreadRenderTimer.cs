using System;
using System.Diagnostics;
using Avalonia.Metadata;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Rendering;

[PrivateApi]
public class UiThreadRenderTimer : DefaultRenderTimer
{
	public override bool RunsInBackground => false;

	public UiThreadRenderTimer(int framesPerSecond)
		: base(framesPerSecond)
	{
	}

	protected override IDisposable StartCore(Action<TimeSpan> tick)
	{
		bool cancelled = false;
		Stopwatch st = Stopwatch.StartNew();
		DispatcherTimer.Run(delegate
		{
			if (cancelled)
			{
				return false;
			}
			tick(st.Elapsed);
			return !cancelled;
		}, TimeSpan.FromSeconds(1.0 / (double)base.FramesPerSecond), DispatcherPriority.UiThreadRender);
		return Disposable.Create(delegate
		{
			cancelled = true;
		});
	}
}
