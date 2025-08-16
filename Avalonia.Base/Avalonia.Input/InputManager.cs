using System;
using Avalonia.Input.Raw;
using Avalonia.Reactive;

namespace Avalonia.Input;

internal class InputManager : IInputManager
{
	private readonly LightweightSubject<RawInputEventArgs> _preProcess = new LightweightSubject<RawInputEventArgs>();

	private readonly LightweightSubject<RawInputEventArgs> _process = new LightweightSubject<RawInputEventArgs>();

	private readonly LightweightSubject<RawInputEventArgs> _postProcess = new LightweightSubject<RawInputEventArgs>();

	public static IInputManager? Instance => AvaloniaLocator.Current.GetService<IInputManager>();

	public IObservable<RawInputEventArgs> PreProcess => _preProcess;

	public IObservable<RawInputEventArgs> Process => _process;

	public IObservable<RawInputEventArgs> PostProcess => _postProcess;

	public void ProcessInput(RawInputEventArgs e)
	{
		_preProcess.OnNext(e);
		e.Device?.ProcessRawEvent(e);
		_process.OnNext(e);
		_postProcess.OnNext(e);
	}
}
