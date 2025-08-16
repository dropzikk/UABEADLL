using System;

namespace Avalonia.Data;

public class IndexerDescriptor : IObservable<object?>, IDescription
{
	public BindingMode Mode { get; set; }

	public BindingPriority Priority { get; set; }

	public AvaloniaProperty? Property { get; set; }

	public AvaloniaObject? Source { get; set; }

	public IObservable<object>? SourceObservable { get; set; }

	public string Description => Source?.GetType().Name + "." + Property?.Name;

	public static IndexerDescriptor operator !(IndexerDescriptor binding)
	{
		return binding.WithMode(BindingMode.TwoWay);
	}

	public static IndexerDescriptor operator ~(IndexerDescriptor binding)
	{
		return binding.WithMode(BindingMode.TwoWay);
	}

	public IndexerDescriptor WithMode(BindingMode mode)
	{
		Mode = mode;
		return this;
	}

	public IndexerDescriptor WithPriority(BindingPriority priority)
	{
		Priority = priority;
		return this;
	}

	public IDisposable Subscribe(IObserver<object?> observer)
	{
		if (SourceObservable == null && Source == null)
		{
			throw new InvalidOperationException("Cannot subscribe to IndexerDescriptor.");
		}
		if ((object)Property == null)
		{
			throw new InvalidOperationException("Cannot subscribe to IndexerDescriptor.");
		}
		return (SourceObservable ?? Source.GetObservable(Property)).Subscribe(observer);
	}
}
