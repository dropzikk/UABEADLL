using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Controls;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMenuProxy : MicroComProxyBase, IAvnMenu, IUnknown, IDisposable
{
	private AvaloniaNativeMenuExporter _exporter;

	private List<__MicroComIAvnMenuItemProxy> _menuItems = new List<__MicroComIAvnMenuItemProxy>();

	private Dictionary<NativeMenuItemBase, __MicroComIAvnMenuItemProxy> _menuItemLookup = new Dictionary<NativeMenuItemBase, __MicroComIAvnMenuItemProxy>();

	internal NativeMenu ManagedMenu { get; private set; }

	protected override int VTableSize => base.VTableSize + 4;

	public void RaiseNeedsUpdate()
	{
		((INativeMenuExporterEventsImplBridge)ManagedMenu).RaiseNeedsUpdate();
		_exporter.UpdateIfNeeded();
	}

	public void RaiseOpening()
	{
		((INativeMenuExporterEventsImplBridge)ManagedMenu).RaiseOpening();
	}

	public void RaiseClosed()
	{
		((INativeMenuExporterEventsImplBridge)ManagedMenu).RaiseClosed();
	}

	public static __MicroComIAvnMenuProxy Create(IAvaloniaNativeFactory factory)
	{
		using MenuEvents menuEvents = new MenuEvents();
		__MicroComIAvnMenuProxy _MicroComIAvnMenuProxy = (__MicroComIAvnMenuProxy)factory.CreateMenu(menuEvents);
		menuEvents.Initialise(_MicroComIAvnMenuProxy);
		return _MicroComIAvnMenuProxy;
	}

	private void RemoveAndDispose(__MicroComIAvnMenuItemProxy item)
	{
		_menuItemLookup.Remove(item.ManagedMenuItem);
		_menuItems.Remove(item);
		RemoveItem(item);
		item.Deinitialize();
		item.Dispose();
	}

	private void MoveExistingTo(int index, __MicroComIAvnMenuItemProxy item)
	{
		_menuItems.Remove(item);
		_menuItems.Insert(index, item);
		RemoveItem(item);
		InsertItem(index, item);
	}

	private __MicroComIAvnMenuItemProxy CreateNewAt(IAvaloniaNativeFactory factory, int index, NativeMenuItemBase item)
	{
		__MicroComIAvnMenuItemProxy _MicroComIAvnMenuItemProxy = CreateNew(factory, item);
		_MicroComIAvnMenuItemProxy.Initialize(item);
		_menuItemLookup.Add(_MicroComIAvnMenuItemProxy.ManagedMenuItem, _MicroComIAvnMenuItemProxy);
		_menuItems.Insert(index, _MicroComIAvnMenuItemProxy);
		InsertItem(index, _MicroComIAvnMenuItemProxy);
		return _MicroComIAvnMenuItemProxy;
	}

	private static __MicroComIAvnMenuItemProxy CreateNew(IAvaloniaNativeFactory factory, NativeMenuItemBase item)
	{
		__MicroComIAvnMenuItemProxy obj = (__MicroComIAvnMenuItemProxy)((item is NativeMenuItemSeparator) ? factory.CreateMenuItemSeparator() : factory.CreateMenuItem());
		obj.ManagedMenuItem = item;
		return obj;
	}

	internal void Initialize(AvaloniaNativeMenuExporter exporter, NativeMenu managedMenu, string title)
	{
		_exporter = exporter;
		ManagedMenu = managedMenu;
		((INotifyCollectionChanged)ManagedMenu.Items).CollectionChanged += OnMenuItemsChanged;
		if (!string.IsNullOrWhiteSpace(title))
		{
			SetTitle(title);
		}
	}

	public void Deinitialise()
	{
		((INotifyCollectionChanged)ManagedMenu.Items).CollectionChanged -= OnMenuItemsChanged;
		foreach (__MicroComIAvnMenuItemProxy menuItem in _menuItems)
		{
			menuItem.Deinitialize();
			menuItem.Dispose();
		}
	}

	internal void Update(IAvaloniaNativeFactory factory, NativeMenu menu)
	{
		if (menu != ManagedMenu)
		{
			throw new ArgumentException("The menu being updated does not match.", "menu");
		}
		for (int i = 0; i < menu.Items.Count; i++)
		{
			__MicroComIAvnMenuItemProxy value;
			if (i >= _menuItems.Count)
			{
				value = CreateNewAt(factory, i, menu.Items[i]);
			}
			else if (menu.Items[i] == _menuItems[i].ManagedMenuItem)
			{
				value = _menuItems[i];
			}
			else if (_menuItemLookup.TryGetValue(menu.Items[i], out value))
			{
				MoveExistingTo(i, value);
			}
			else
			{
				value = CreateNewAt(factory, i, menu.Items[i]);
			}
			if (menu.Items[i] is NativeMenuItem item)
			{
				value.Update(_exporter, factory, item);
			}
		}
		while (_menuItems.Count > menu.Items.Count)
		{
			RemoveAndDispose(_menuItems[_menuItems.Count - 1]);
		}
	}

	private void OnMenuItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		_exporter.QueueReset();
	}

	public unsafe void InsertItem(int index, IAvnMenuItem item)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, index, MicroComRuntime.GetNativePointer(item));
		if (num != 0)
		{
			throw new COMException("InsertItem failed", num);
		}
	}

	public unsafe void RemoveItem(IAvnMenuItem item)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(item));
		if (num != 0)
		{
			throw new COMException("RemoveItem failed", num);
		}
	}

	public unsafe void SetTitle(string utf8String)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(utf8String) + 1];
		Encoding.UTF8.GetBytes(utf8String, 0, utf8String.Length, array, 0);
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, ptr);
		}
		if (num != 0)
		{
			throw new COMException("SetTitle failed", num);
		}
	}

	public unsafe void Clear()
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV);
		if (num != 0)
		{
			throw new COMException("Clear failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMenu), new Guid("a7724dc1-cf6b-4fa8-9d23-228bf2593edc"), (IntPtr p, bool owns) => new __MicroComIAvnMenuProxy(p, owns));
	}

	protected __MicroComIAvnMenuProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
