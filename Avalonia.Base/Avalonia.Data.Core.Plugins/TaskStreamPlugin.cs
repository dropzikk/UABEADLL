using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Reactive;

namespace Avalonia.Data.Core.Plugins;

[UnconditionalSuppressMessage("Trimming", "IL3050", Justification = "This method is not supported by NativeAOT.")]
internal class TaskStreamPlugin : IStreamPlugin
{
	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	public virtual bool Match(WeakReference<object?> reference)
	{
		reference.TryGetTarget(out object target);
		return target is Task;
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	public virtual IObservable<object?> Start(WeakReference<object?> reference)
	{
		reference.TryGetTarget(out object target);
		Task task = target as Task;
		if (task != null && task.GetType().GetRuntimeProperty("Result") != null)
		{
			TaskStatus status = task.Status;
			if (status == TaskStatus.RanToCompletion || status == TaskStatus.Faulted)
			{
				return HandleCompleted(task);
			}
			LightweightSubject<object?> subject = new LightweightSubject<object>();
			task.ContinueWith((Task x) => HandleCompleted(task).Subscribe(subject), TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(continueOnCapturedContext: false);
			return subject;
		}
		return Observable.Empty<object>();
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	private static IObservable<object?> HandleCompleted(Task task)
	{
		PropertyInfo runtimeProperty = task.GetType().GetRuntimeProperty("Result");
		if (runtimeProperty != null)
		{
			return task.Status switch
			{
				TaskStatus.RanToCompletion => Observable.Return(runtimeProperty.GetValue(task)), 
				TaskStatus.Faulted => Observable.Return(new BindingNotification(task.Exception, BindingErrorType.Error)), 
				_ => throw new AvaloniaInternalException("HandleCompleted called for non-completed Task."), 
			};
		}
		return Observable.Empty<object>();
	}
}
