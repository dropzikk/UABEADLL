using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Reactive;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnMenuItemProxy : MicroComProxyBase, IAvnMenuItem, IUnknown, IDisposable
{
	private __MicroComIAvnMenuProxy _subMenu;

	private CompositeDisposable _propertyDisposables = new CompositeDisposable();

	private IDisposable _currentActionDisposable;

	public NativeMenuItemBase ManagedMenuItem { get; set; }

	protected override int VTableSize => base.VTableSize + 7;

	private void UpdateTitle(string title)
	{
		SetTitle(title ?? "");
	}

	private void UpdateIsChecked(bool isChecked)
	{
		SetIsChecked(isChecked.AsComBool());
	}

	private void UpdateToggleType(NativeMenuItemToggleType toggleType)
	{
		SetToggleType((AvnMenuItemToggleType)toggleType);
	}

	private unsafe void UpdateIcon(IBitmap icon)
	{
		if (icon == null)
		{
			SetIcon(null, IntPtr.Zero);
			return;
		}
		using MemoryStream memoryStream = new MemoryStream();
		icon.Save(memoryStream, null);
		byte[] array = memoryStream.ToArray();
		fixed (byte* ptr = array)
		{
			void* data = ptr;
			SetIcon(data, new IntPtr(array.Length));
		}
	}

	private void UpdateGesture(KeyGesture gesture)
	{
		AvnInputModifiers modifiers = (AvnInputModifiers)((!(gesture == null)) ? gesture.KeyModifiers : KeyModifiers.None);
		AvnKey key = (AvnKey)((!(gesture == null)) ? gesture.Key : Key.None);
		SetGesture(key, modifiers);
	}

	private void UpdateAction(NativeMenuItem item)
	{
		_currentActionDisposable?.Dispose();
		PredicateCallback action = new PredicateCallback(() => (item.Command != null || item.HasClickHandlers) && item.IsEnabled);
		MenuActionCallback callback = new MenuActionCallback(delegate
		{
			((INativeMenuItemExporterEventsImplBridge)item)?.RaiseClicked();
		});
		_currentActionDisposable = Disposable.Create(delegate
		{
			action.Dispose();
			callback.Dispose();
		});
		SetAction(action, callback);
	}

	internal void Initialize(NativeMenuItemBase nativeMenuItem)
	{
		ManagedMenuItem = nativeMenuItem;
		if (ManagedMenuItem is NativeMenuItem nativeMenuItem2)
		{
			UpdateTitle(nativeMenuItem2.Header);
			UpdateGesture(nativeMenuItem2.Gesture);
			UpdateAction(ManagedMenuItem as NativeMenuItem);
			UpdateToggleType(nativeMenuItem2.ToggleType);
			UpdateIcon(nativeMenuItem2.Icon);
			UpdateIsChecked(nativeMenuItem2.IsChecked);
			_propertyDisposables.Add(ManagedMenuItem.GetObservable(NativeMenuItem.HeaderProperty).Subscribe(delegate(string x)
			{
				UpdateTitle(x);
			}));
			_propertyDisposables.Add(ManagedMenuItem.GetObservable(NativeMenuItem.GestureProperty).Subscribe(delegate(KeyGesture x)
			{
				UpdateGesture(x);
			}));
			_propertyDisposables.Add(ManagedMenuItem.GetObservable(NativeMenuItem.CommandProperty).Subscribe(delegate
			{
				UpdateAction(ManagedMenuItem as NativeMenuItem);
			}));
			_propertyDisposables.Add(ManagedMenuItem.GetObservable(NativeMenuItem.ToggleTypeProperty).Subscribe(delegate(NativeMenuItemToggleType x)
			{
				UpdateToggleType(x);
			}));
			_propertyDisposables.Add(ManagedMenuItem.GetObservable(NativeMenuItem.IsCheckedProperty).Subscribe(delegate(bool x)
			{
				UpdateIsChecked(x);
			}));
			_propertyDisposables.Add(ManagedMenuItem.GetObservable(NativeMenuItem.IconProperty).Subscribe(delegate(Bitmap x)
			{
				UpdateIcon(x);
			}));
		}
	}

	internal void Deinitialize()
	{
		if (_subMenu != null)
		{
			SetSubMenu(null);
			_subMenu.Deinitialise();
			_subMenu.Dispose();
			_subMenu = null;
		}
		_propertyDisposables?.Dispose();
		_currentActionDisposable?.Dispose();
	}

	internal void Update(AvaloniaNativeMenuExporter exporter, IAvaloniaNativeFactory factory, NativeMenuItem item)
	{
		if (item != ManagedMenuItem)
		{
			throw new ArgumentException("The item does not match the menuitem being updated.", "item");
		}
		if (item.Menu != null)
		{
			if (_subMenu == null)
			{
				_subMenu = __MicroComIAvnMenuProxy.Create(factory);
				if (item.Menu.GetValue(MacOSNativeMenuCommands.IsServicesSubmenuProperty))
				{
					factory.SetServicesMenu(_subMenu);
				}
				_subMenu.Initialize(exporter, item.Menu, item.Header);
				SetSubMenu(_subMenu);
			}
			_subMenu.Update(factory, item.Menu);
		}
		if (item.Menu == null && _subMenu != null)
		{
			_subMenu.Deinitialise();
			_subMenu.Dispose();
			SetSubMenu(null);
		}
	}

	public unsafe void SetSubMenu(IAvnMenu menu)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, MicroComRuntime.GetNativePointer(menu));
		if (num != 0)
		{
			throw new COMException("SetSubMenu failed", num);
		}
	}

	public unsafe void SetTitle(string utf8String)
	{
		byte[] array = new byte[Encoding.UTF8.GetByteCount(utf8String) + 1];
		Encoding.UTF8.GetBytes(utf8String, 0, utf8String.Length, array, 0);
		int num;
		fixed (byte* ptr = array)
		{
			num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, ptr);
		}
		if (num != 0)
		{
			throw new COMException("SetTitle failed", num);
		}
	}

	public unsafe void SetGesture(AvnKey key, AvnInputModifiers modifiers)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnKey, AvnInputModifiers, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, key, modifiers);
		if (num != 0)
		{
			throw new COMException("SetGesture failed", num);
		}
	}

	public unsafe void SetAction(IAvnPredicateCallback predicate, IAvnActionCallback callback)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, MicroComRuntime.GetNativePointer(predicate), MicroComRuntime.GetNativePointer(callback));
		if (num != 0)
		{
			throw new COMException("SetAction failed", num);
		}
	}

	public unsafe void SetIsChecked(int isChecked)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, int, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, isChecked);
		if (num != 0)
		{
			throw new COMException("SetIsChecked failed", num);
		}
	}

	public unsafe void SetToggleType(AvnMenuItemToggleType toggleType)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, AvnMenuItemToggleType, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, toggleType);
		if (num != 0)
		{
			throw new COMException("SetToggleType failed", num);
		}
	}

	public unsafe void SetIcon(void* data, IntPtr length)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, data, length);
		if (num != 0)
		{
			throw new COMException("SetIcon failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IAvnMenuItem), new Guid("f890219a-1720-4cd5-9a26-cd95fccbf53c"), (IntPtr p, bool owns) => new __MicroComIAvnMenuItemProxy(p, owns));
	}

	protected __MicroComIAvnMenuItemProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
