using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Avalonia.Reactive;

namespace Avalonia.Data.Core.Plugins;

[UnconditionalSuppressMessage("Trimming", "IL3050", Justification = "This method is not supported by NativeAOT.")]
internal class ObservableStreamPlugin : IStreamPlugin
{
	private static MethodInfo? s_observableGeneric;

	private static MethodInfo? s_observableSelect;

	[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, "Avalonia.Data.Core.Plugins.ObservableStreamPlugin", "Avalonia.Base")]
	public ObservableStreamPlugin()
	{
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	public virtual bool Match(WeakReference<object?> reference)
	{
		reference.TryGetTarget(out object target);
		return target?.GetType().GetInterfaces().Any((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IObservable<>)) ?? false;
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	public virtual IObservable<object?> Start(WeakReference<object?> reference)
	{
		if (!reference.TryGetTarget(out object target) || target == null)
		{
			return Observable.Empty<object>();
		}
		if (target is IObservable<object> result)
		{
			return result;
		}
		return (IObservable<object>)GetBoxObservable(target.GetType().GetInterfaces().First((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IObservable<>))
			.GetGenericArguments()[0]).Invoke(null, new object[1] { target });
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	private static MethodInfo GetBoxObservable(Type source)
	{
		return (s_observableGeneric ?? (s_observableGeneric = GetBoxObservable())).MakeGenericMethod(source);
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	private static MethodInfo GetBoxObservable()
	{
		object obj = s_observableSelect;
		if (obj == null)
		{
			obj = typeof(ObservableStreamPlugin).GetMethod("BoxObservable", BindingFlags.Static | BindingFlags.NonPublic) ?? throw new InvalidOperationException("BoxObservable method was not found.");
			s_observableSelect = (MethodInfo?)obj;
		}
		return (MethodInfo)obj;
	}

	private static IObservable<object?> BoxObservable<T>(IObservable<T> source)
	{
		return source.Select((Func<T, object>)((T v) => v));
	}
}
