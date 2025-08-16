using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class PopupImpl : WindowBaseImpl, IPopupImpl, IWindowBaseImpl, ITopLevelImpl, IOptionalFeatureProvider, IDisposable
{
	private class PopupEvents : WindowBaseEvents, IAvnWindowEvents, IAvnWindowBaseEvents, IUnknown, IDisposable
	{
		private readonly PopupImpl _parent;

		public PopupEvents(PopupImpl parent)
			: base(parent)
		{
			_parent = parent;
		}

		public void GotInputWhenDisabled()
		{
		}

		int IAvnWindowEvents.Closing()
		{
			return Extensions.AsComBool(b: true);
		}

		void IAvnWindowEvents.WindowStateChanged(AvnWindowState state)
		{
		}
	}

	private readonly IWindowBaseImpl _parent;

	public IPopupPositioner PopupPositioner { get; }

	public PopupImpl(IAvaloniaNativeFactory factory, IWindowBaseImpl parent)
		: base(factory)
	{
		_parent = parent;
		using (PopupEvents cb = new PopupEvents(this))
		{
			Init(factory.CreatePopup(cb), factory.CreateScreens());
		}
		PopupPositioner = new ManagedPopupPositioner(new ManagedPopupPositionerPopupImplHelper(parent, MoveResize));
	}

	private void MoveResize(PixelPoint position, Size size, double scaling)
	{
		base.Position = position;
		Resize(size, WindowResizeReason.Layout);
	}

	public override void Show(bool activate, bool isDialog)
	{
		IWindowBaseImpl parent;
		for (parent = _parent; parent is PopupImpl popupImpl; parent = popupImpl._parent)
		{
		}
		if (parent is WindowImpl windowImpl)
		{
			windowImpl.Native.TakeFocusFromChildren();
		}
		base.Show(activate: false, isDialog);
	}

	public override IPopupImpl CreatePopup()
	{
		return new PopupImpl(_factory, this);
	}

	public void SetWindowManagerAddShadowHint(bool enabled)
	{
	}
}
