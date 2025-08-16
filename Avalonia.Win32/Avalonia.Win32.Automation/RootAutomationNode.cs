using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Platform;
using Avalonia.Win32.Interop.Automation;

namespace Avalonia.Win32.Automation;

[RequiresUnreferencedCode("Requires .NET COM interop")]
internal class RootAutomationNode : AutomationNode, IRawElementProviderFragmentRoot, IRawElementProviderFragment, IRawElementProviderSimple, IRawElementProviderAdviseEvents
{
	private int _raiseFocusChanged;

	public override IRawElementProviderFragmentRoot? FragmentRoot => this;

	public new IRootProvider Peer { get; }

	public IWindowBaseImpl? WindowImpl => Peer.PlatformImpl as IWindowBaseImpl;

	public override IRawElementProviderSimple? HostRawElementProvider
	{
		get
		{
			IntPtr intPtr = WindowImpl?.Handle.Handle ?? IntPtr.Zero;
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			Marshal.ThrowExceptionForHR(UiaCoreProviderApi.UiaHostProviderFromHwnd(intPtr, out IRawElementProviderSimple provider));
			return provider;
		}
	}

	public RootAutomationNode(AutomationPeer peer)
		: base(peer)
	{
		Peer = base.Peer.GetProvider<IRootProvider>() ?? throw new AvaloniaInternalException("Attempt to create RootAutomationNode from peer which does not implement IRootProvider.");
		Peer.FocusChanged += FocusChanged;
	}

	public IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
	{
		if (WindowImpl == null)
		{
			return null;
		}
		Point p = WindowImpl.PointToClient(new PixelPoint((int)x, (int)y));
		return AutomationNode.GetOrCreate(InvokeSync(() => Peer.GetPeerFromPoint(p)));
	}

	public IRawElementProviderFragment? GetFocus()
	{
		return AutomationNode.GetOrCreate(InvokeSync(() => Peer.GetFocus()));
	}

	void IRawElementProviderAdviseEvents.AdviseEventAdded(int eventId, int[] properties)
	{
		if (eventId == 20005)
		{
			_raiseFocusChanged++;
		}
	}

	void IRawElementProviderAdviseEvents.AdviseEventRemoved(int eventId, int[] properties)
	{
		if (eventId == 20005)
		{
			_raiseFocusChanged--;
		}
	}

	protected void RaiseFocusChanged(AutomationNode? focused)
	{
		if (_raiseFocusChanged > 0)
		{
			UiaCoreProviderApi.UiaRaiseAutomationEvent(focused, 20005);
		}
	}

	public void FocusChanged(object? sender, EventArgs e)
	{
		RaiseFocusChanged(AutomationNode.GetOrCreate(Peer.GetFocus()));
	}

	public Rect ToScreen(Rect rect)
	{
		if (WindowImpl == null)
		{
			return default(Rect);
		}
		return new PixelRect(WindowImpl.PointToScreen(rect.TopLeft), WindowImpl.PointToScreen(rect.BottomRight)).ToRect(1.0);
	}
}
