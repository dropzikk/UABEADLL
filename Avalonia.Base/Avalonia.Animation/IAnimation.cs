using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Animation;

[NotClientImplementable]
public interface IAnimation
{
	internal IDisposable Apply(Animatable control, IClock? clock, IObservable<bool> match, Action? onComplete = null);

	internal Task RunAsync(Animatable control, IClock clock, CancellationToken cancellationToken = default(CancellationToken));
}
