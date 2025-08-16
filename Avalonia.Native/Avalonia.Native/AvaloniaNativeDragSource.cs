using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Native.Interop;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvaloniaNativeDragSource : IPlatformDragSource
{
	private class DndCallback : NativeCallbackBase, IAvnDndResultCallback, IUnknown, IDisposable
	{
		private TaskCompletionSource<DragDropEffects> _tcs;

		public DndCallback(TaskCompletionSource<DragDropEffects> tcs)
		{
			_tcs = tcs;
		}

		public void OnDragAndDropComplete(AvnDragDropEffects effect)
		{
			_tcs?.TrySetResult((DragDropEffects)effect);
			_tcs = null;
		}
	}

	private readonly IAvaloniaNativeFactory _factory;

	public AvaloniaNativeDragSource(IAvaloniaNativeFactory factory)
	{
		_factory = factory;
	}

	public Task<DragDropEffects> DoDragDrop(PointerEventArgs triggerEvent, IDataObject data, DragDropEffects allowedEffects)
	{
		TopLevel topLevel = TopLevel.GetTopLevel(triggerEvent.Source as Visual);
		if (!(topLevel?.PlatformImpl is WindowBaseImpl windowBaseImpl))
		{
			throw new ArgumentException();
		}
		triggerEvent.Pointer.Capture(null);
		TaskCompletionSource<DragDropEffects> taskCompletionSource = new TaskCompletionSource<DragDropEffects>();
		IAvnClipboard avnClipboard = _factory.CreateDndClipboard();
		using (ClipboardImpl clipboardImpl = new ClipboardImpl(avnClipboard))
		{
			using DndCallback callback = new DndCallback(taskCompletionSource);
			if (data.Contains(DataFormats.Text))
			{
				clipboardImpl.SetTextAsync(data.GetText()).Wait();
			}
			windowBaseImpl.BeginDraggingSession((AvnDragDropEffects)allowedEffects, triggerEvent.GetPosition(topLevel).ToAvnPoint(), avnClipboard, callback, GCHandle.ToIntPtr(GCHandle.Alloc(data)));
		}
		return taskCompletionSource.Task;
	}
}
