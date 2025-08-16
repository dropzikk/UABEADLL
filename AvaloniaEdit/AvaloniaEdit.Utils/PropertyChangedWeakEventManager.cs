using System.ComponentModel;

namespace AvaloniaEdit.Utils;

internal sealed class PropertyChangedWeakEventManager : WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>
{
	protected override void StartListening(INotifyPropertyChanged source)
	{
		source.PropertyChanged += WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.DeliverEvent;
	}

	protected override void StopListening(INotifyPropertyChanged source)
	{
		source.PropertyChanged -= WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>.DeliverEvent;
	}
}
