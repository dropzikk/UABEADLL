using System.ComponentModel;

namespace Avalonia.Rendering;

public class RendererDiagnostics : INotifyPropertyChanged
{
	private RendererDebugOverlays _debugOverlays;

	private LayoutPassTiming _lastLayoutPassTiming;

	private PropertyChangedEventArgs? _debugOverlaysChangedEventArgs;

	private PropertyChangedEventArgs? _lastLayoutPassTimingChangedEventArgs;

	public RendererDebugOverlays DebugOverlays
	{
		get
		{
			return _debugOverlays;
		}
		set
		{
			if (_debugOverlays != value)
			{
				_debugOverlays = value;
				OnPropertyChanged(_debugOverlaysChangedEventArgs ?? (_debugOverlaysChangedEventArgs = new PropertyChangedEventArgs("DebugOverlays")));
			}
		}
	}

	internal LayoutPassTiming LastLayoutPassTiming
	{
		get
		{
			return _lastLayoutPassTiming;
		}
		set
		{
			if (!_lastLayoutPassTiming.Equals(value))
			{
				_lastLayoutPassTiming = value;
				OnPropertyChanged(_lastLayoutPassTimingChangedEventArgs ?? (_lastLayoutPassTimingChangedEventArgs = new PropertyChangedEventArgs("LastLayoutPassTiming")));
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
	{
		this.PropertyChanged?.Invoke(this, args);
	}
}
