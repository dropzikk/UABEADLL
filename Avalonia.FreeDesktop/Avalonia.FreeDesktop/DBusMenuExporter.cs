using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Threading;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop;

internal class DBusMenuExporter
{
	private class DBusMenuExporterImpl : ComCanonicalDbusmenu, ITopLevelNativeMenuExporter, INativeMenuExporter, IDisposable
	{
		private readonly Dictionary<int, NativeMenuItemBase> _idsToItems = new Dictionary<int, NativeMenuItemBase>();

		private readonly Dictionary<NativeMenuItemBase, int> _itemsToIds = new Dictionary<NativeMenuItemBase, int>();

		private readonly HashSet<NativeMenu> _menus = new HashSet<NativeMenu>();

		private readonly uint _xid;

		private readonly bool _appMenu = true;

		private ComCanonicalAppMenuRegistrar? _registrar;

		private NativeMenu? _menu;

		private bool _disposed;

		private uint _revision = 1u;

		private bool _resetQueued;

		private int _nextId = 1;

		private static readonly string[] s_allProperties = new string[9] { "type", "label", "enabled", "visible", "shortcut", "toggle-type", "children-display", "toggle-state", "icon-data" };

		protected override Connection Connection { get; }

		public override string Path { get; }

		public bool IsNativeMenuExported { get; private set; }

		public event EventHandler? OnIsNativeMenuExportedChanged;

		public DBusMenuExporterImpl(Connection connection, IntPtr xid)
			: this()
		{
			Connection = connection;
			_xid = (uint)xid.ToInt32();
			Path = GenerateDBusMenuObjPath;
			SetNativeMenu(new NativeMenu());
			InitializeAsync();
		}

		public DBusMenuExporterImpl(Connection connection, string path)
			: this()
		{
			Connection = connection;
			_appMenu = false;
			Path = path;
			SetNativeMenu(new NativeMenu());
			InitializeAsync();
		}

		private DBusMenuExporterImpl()
		{
			base.BackingProperties.Status = string.Empty;
			base.BackingProperties.TextDirection = string.Empty;
			base.BackingProperties.IconThemePath = Array.Empty<string>();
		}

		protected override ValueTask<(uint revision, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) layout)> OnGetLayoutAsync(int parentId, int recursionDepth, string[] propertyNames)
		{
			(NativeMenuItemBase, NativeMenu) menu = GetMenu(parentId);
			(int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) layout = GetLayout(menu.Item1, menu.Item2, recursionDepth, propertyNames);
			if (!IsNativeMenuExported)
			{
				IsNativeMenuExported = true;
				this.OnIsNativeMenuExportedChanged?.Invoke(this, EventArgs.Empty);
			}
			return new ValueTask<(uint, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]))>((_revision, layout));
		}

		protected override ValueTask<(int, Dictionary<string, DBusVariantItem>)[]> OnGetGroupPropertiesAsync(int[] ids, string[] propertyNames)
		{
			return new ValueTask<(int, Dictionary<string, DBusVariantItem>)[]>(ids.Select((int id) => (id: id, GetProperties(GetMenu(id), propertyNames))).ToArray());
		}

		protected override ValueTask<DBusVariantItem> OnGetPropertyAsync(int id, string name)
		{
			return new ValueTask<DBusVariantItem>(GetProperty(GetMenu(id), name) ?? new DBusVariantItem("i", new DBusInt32Item(0)));
		}

		protected override ValueTask OnEventAsync(int id, string eventId, DBusVariantItem data, uint timestamp)
		{
			HandleEvent(id, eventId);
			return default(ValueTask);
		}

		protected override ValueTask<int[]> OnEventGroupAsync((int, string, DBusVariantItem, uint)[] events)
		{
			for (int i = 0; i < events.Length; i++)
			{
				(int, string, DBusVariantItem, uint) tuple = events[i];
				HandleEvent(tuple.Item1, tuple.Item2);
			}
			return new ValueTask<int[]>(Array.Empty<int>());
		}

		protected override ValueTask<bool> OnAboutToShowAsync(int id)
		{
			return new ValueTask<bool>(result: false);
		}

		protected override ValueTask<(int[] updatesNeeded, int[] idErrors)> OnAboutToShowGroupAsync(int[] ids)
		{
			return new ValueTask<(int[], int[])>((Array.Empty<int>(), Array.Empty<int>()));
		}

		private async Task InitializeAsync()
		{
			Connection.AddMethodHandler(this);
			if (!_appMenu)
			{
				return;
			}
			_registrar = new ComCanonicalAppMenuRegistrar(Connection, "com.canonical.AppMenu.Registrar", "/com/canonical/AppMenu/Registrar");
			try
			{
				if (!_disposed)
				{
					await _registrar.RegisterWindowAsync(_xid, Path);
				}
			}
			catch
			{
				_registrar = null;
			}
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_registrar?.UnregisterWindowAsync(_xid);
			}
		}

		public void SetNativeMenu(NativeMenu? menu)
		{
			if (menu == null)
			{
				menu = new NativeMenu();
			}
			if (_menu != null)
			{
				((INotifyCollectionChanged)_menu.Items).CollectionChanged -= OnMenuItemsChanged;
			}
			_menu = menu;
			((INotifyCollectionChanged)_menu.Items).CollectionChanged += OnMenuItemsChanged;
			DoLayoutReset();
		}

		private void DoLayoutReset()
		{
			_resetQueued = false;
			foreach (NativeMenuItemBase value in _idsToItems.Values)
			{
				value.PropertyChanged -= OnItemPropertyChanged;
			}
			foreach (NativeMenu menu in _menus)
			{
				((INotifyCollectionChanged)menu.Items).CollectionChanged -= OnMenuItemsChanged;
			}
			_menus.Clear();
			_idsToItems.Clear();
			_itemsToIds.Clear();
			_revision++;
			EmitLayoutUpdated(_revision, 0);
		}

		private void QueueReset()
		{
			if (!_resetQueued)
			{
				_resetQueued = true;
				Dispatcher.UIThread.Post(DoLayoutReset, DispatcherPriority.Background);
			}
		}

		private (NativeMenuItemBase? item, NativeMenu? menu) GetMenu(int id)
		{
			if (id == 0)
			{
				return (item: null, menu: _menu);
			}
			_idsToItems.TryGetValue(id, out NativeMenuItemBase value);
			return (item: value, menu: (value as NativeMenuItem)?.Menu);
		}

		private void EnsureSubscribed(NativeMenu? menu)
		{
			if (menu != null && _menus.Add(menu))
			{
				((INotifyCollectionChanged)menu.Items).CollectionChanged += OnMenuItemsChanged;
			}
		}

		private int GetId(NativeMenuItemBase item)
		{
			if (_itemsToIds.TryGetValue(item, out var value))
			{
				return value;
			}
			value = _nextId++;
			_idsToItems[value] = item;
			_itemsToIds[item] = value;
			item.PropertyChanged += OnItemPropertyChanged;
			if (item is NativeMenuItem nativeMenuItem)
			{
				EnsureSubscribed(nativeMenuItem.Menu);
			}
			return value;
		}

		private void OnMenuItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			QueueReset();
		}

		private void OnItemPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			QueueReset();
		}

		private static DBusVariantItem? GetProperty((NativeMenuItemBase? item, NativeMenu? menu) i, string name)
		{
			var (nativeMenuItemBase, nativeMenu) = i;
			if (nativeMenuItemBase is NativeMenuItemSeparator)
			{
				if (name == "type")
				{
					return new DBusVariantItem("s", new DBusStringItem("separator"));
				}
			}
			else if (nativeMenuItemBase is NativeMenuItem nativeMenuItem)
			{
				switch (name)
				{
				case "type":
					return null;
				case "label":
					return new DBusVariantItem("s", new DBusStringItem(nativeMenuItem.Header ?? "<null>"));
				case "enabled":
					if (nativeMenuItem.Menu != null && nativeMenuItem.Menu.Items.Count == 0)
					{
						return new DBusVariantItem("b", new DBusBoolItem(value: false));
					}
					if (!nativeMenuItem.IsEnabled)
					{
						return new DBusVariantItem("b", new DBusBoolItem(value: false));
					}
					return null;
				case "shortcut":
				{
					if ((object)nativeMenuItem.Gesture == null)
					{
						return null;
					}
					if (nativeMenuItem.Gesture.KeyModifiers == KeyModifiers.None)
					{
						return null;
					}
					List<DBusItem> list = new List<DBusItem>();
					KeyGesture? gesture = nativeMenuItem.Gesture;
					if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Control))
					{
						list.Add(new DBusStringItem("Control"));
					}
					if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Alt))
					{
						list.Add(new DBusStringItem("Alt"));
					}
					if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Shift))
					{
						list.Add(new DBusStringItem("Shift"));
					}
					if (gesture.KeyModifiers.HasAllFlags(KeyModifiers.Meta))
					{
						list.Add(new DBusStringItem("Super"));
					}
					list.Add(new DBusStringItem(nativeMenuItem.Gesture.Key.ToString()));
					return new DBusVariantItem("aas", new DBusArrayItem(DBusType.Array, new DBusArrayItem[1]
					{
						new DBusArrayItem(DBusType.String, list)
					}));
				}
				case "toggle-type":
					if (nativeMenuItem.ToggleType == NativeMenuItemToggleType.CheckBox)
					{
						return new DBusVariantItem("s", new DBusStringItem("checkmark"));
					}
					if (nativeMenuItem.ToggleType == NativeMenuItemToggleType.Radio)
					{
						return new DBusVariantItem("s", new DBusStringItem("radio"));
					}
					break;
				}
				if (name == "toggle-state" && nativeMenuItem.ToggleType != 0)
				{
					return new DBusVariantItem("i", new DBusInt32Item(nativeMenuItem.IsChecked ? 1 : 0));
				}
				if (name == "icon-data" && nativeMenuItem.Icon != null)
				{
					IPlatformIconLoader service = AvaloniaLocator.Current.GetService<IPlatformIconLoader>();
					if (service != null)
					{
						IWindowIconImpl windowIconImpl = service.LoadIcon(nativeMenuItem.Icon.PlatformImpl.Item);
						using MemoryStream memoryStream = new MemoryStream();
						windowIconImpl.Save(memoryStream);
						return new DBusVariantItem("ay", new DBusArrayItem(DBusType.Byte, from x in memoryStream.ToArray()
							select new DBusByteItem(x)));
					}
				}
				if (name == "children-display")
				{
					if (nativeMenu == null)
					{
						return null;
					}
					return new DBusVariantItem("s", new DBusStringItem("submenu"));
				}
			}
			return null;
		}

		private static Dictionary<string, DBusVariantItem> GetProperties((NativeMenuItemBase? item, NativeMenu? menu) i, string[] names)
		{
			if (names.Length == 0)
			{
				names = s_allProperties;
			}
			Dictionary<string, DBusVariantItem> dictionary = new Dictionary<string, DBusVariantItem>();
			string[] array = names;
			foreach (string text in array)
			{
				DBusVariantItem property = GetProperty(i, text);
				if (property != null)
				{
					dictionary.Add(text, property);
				}
			}
			return dictionary;
		}

		private (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) GetLayout(NativeMenuItemBase? item, NativeMenu? menu, int depth, string[] propertyNames)
		{
			int item2 = ((item != null) ? GetId(item) : 0);
			Dictionary<string, DBusVariantItem> properties = GetProperties((item: item, menu: menu), propertyNames);
			DBusVariantItem[] array = ((depth == 0 || menu == null) ? Array.Empty<DBusVariantItem>() : new DBusVariantItem[menu.Items.Count]);
			if (menu != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					NativeMenuItemBase nativeMenuItemBase = menu.Items[i];
					(int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) layout = GetLayout(nativeMenuItemBase, (nativeMenuItemBase as NativeMenuItem)?.Menu, (depth == -1) ? (-1) : (depth - 1), propertyNames);
					array[i] = new DBusVariantItem("(ia{sv}av)", new DBusStructItem(new DBusItem[3]
					{
						new DBusInt32Item(layout.Item1),
						new DBusArrayItem(DBusType.DictEntry, layout.Item2.Select((KeyValuePair<string, DBusVariantItem> x) => new DBusDictEntryItem(new DBusStringItem(x.Key), x.Value))),
						new DBusArrayItem(DBusType.Variant, layout.Item3)
					}));
				}
			}
			return (item2, properties, array);
		}

		private void HandleEvent(int id, string eventId)
		{
			if (eventId == "clicked" && GetMenu(id).item is NativeMenuItem { IsEnabled: not false } nativeMenuItem)
			{
				INativeMenuItemExporterEventsImplBridge nativeMenuItemExporterEventsImplBridge = nativeMenuItem;
				nativeMenuItemExporterEventsImplBridge.RaiseClicked();
			}
		}
	}

	public static string GenerateDBusMenuObjPath => $"/net/avaloniaui/dbusmenu/{Guid.NewGuid():N}";

	public static ITopLevelNativeMenuExporter? TryCreateTopLevelNativeMenu(IntPtr xid)
	{
		if (DBusHelper.Connection != null)
		{
			return new DBusMenuExporterImpl(DBusHelper.Connection, xid);
		}
		return null;
	}

	public static INativeMenuExporter TryCreateDetachedNativeMenu(string path, Connection currentConnection)
	{
		return new DBusMenuExporterImpl(currentConnection, path);
	}
}
