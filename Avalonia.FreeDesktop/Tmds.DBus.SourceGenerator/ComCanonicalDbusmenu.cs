using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal abstract class ComCanonicalDbusmenu : IMethodHandler
{
	public class Properties
	{
		public uint Version { get; set; }

		public string TextDirection { get; set; }

		public string Status { get; set; }

		public string[] IconThemePath { get; set; }
	}

	private SynchronizationContext? _synchronizationContext;

	protected abstract Connection Connection { get; }

	public abstract string Path { get; }

	public Properties BackingProperties { get; } = new Properties();

	public ComCanonicalDbusmenu(bool emitOnCapturedContext = true)
	{
		if (emitOnCapturedContext)
		{
			_synchronizationContext = SynchronizationContext.Current;
		}
	}

	protected abstract ValueTask<(uint revision, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) layout)> OnGetLayoutAsync(int parentId, int recursionDepth, string[] propertyNames);

	protected abstract ValueTask<(int, Dictionary<string, DBusVariantItem>)[]> OnGetGroupPropertiesAsync(int[] ids, string[] propertyNames);

	protected abstract ValueTask<DBusVariantItem> OnGetPropertyAsync(int id, string name);

	protected abstract ValueTask OnEventAsync(int id, string eventId, DBusVariantItem data, uint timestamp);

	protected abstract ValueTask<int[]> OnEventGroupAsync((int, string, DBusVariantItem, uint)[] events);

	protected abstract ValueTask<bool> OnAboutToShowAsync(int id);

	protected abstract ValueTask<(int[] updatesNeeded, int[] idErrors)> OnAboutToShowGroupAsync(int[] ids);

	protected void EmitItemsPropertiesUpdated((int, Dictionary<string, DBusVariantItem>)[] updatedProps, (int, string[])[] removedProps)
	{
		MessageWriter writer = Connection.GetMessageWriter();
		writer.WriteSignalHeader(null, Path, "com.canonical.dbusmenu", "ItemsPropertiesUpdated", "a(ia{sv})a(ias)");
		writer.WriteArray_ariaesvz(updatedProps);
		writer.WriteArray_ariasz(removedProps);
		Connection.TrySendMessage(writer.CreateMessage());
		writer.Dispose();
	}

	protected void EmitLayoutUpdated(uint revision, int parent)
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "com.canonical.dbusmenu", "LayoutUpdated", "ui");
		messageWriter.WriteUInt32(revision);
		messageWriter.WriteInt32(parent);
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	protected void EmitItemActivationRequested(int id, uint timestamp)
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "com.canonical.dbusmenu", "ItemActivationRequested", "iu");
		messageWriter.WriteInt32(id);
		messageWriter.WriteUInt32(timestamp);
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	public bool RunMethodHandlerSynchronously(Message message)
	{
		return true;
	}

	public async ValueTask HandleMethodAsync(MethodContext context)
	{
		string interfaceAsString = context.Request.InterfaceAsString;
		string signatureAsString;
		string memberAsString;
		if (!(interfaceAsString == "com.canonical.dbusmenu"))
		{
			if (!(interfaceAsString == "org.freedesktop.DBus.Properties"))
			{
				return;
			}
			memberAsString = context.Request.MemberAsString;
			signatureAsString = context.Request.SignatureAsString;
			if (!(memberAsString == "Get"))
			{
				if (memberAsString == "GetAll" && signatureAsString == "s")
				{
					Reply();
				}
			}
			else if (signatureAsString == "ss")
			{
				Reply();
			}
			return;
		}
		signatureAsString = context.Request.MemberAsString;
		memberAsString = context.Request.SignatureAsString;
		switch (signatureAsString)
		{
		case "GetLayout":
		{
			if (!(memberAsString == "iias"))
			{
				break;
			}
			int parentId = default(int);
			int recursionDepth = default(int);
			string[] propertyNames2 = default(string[]);
			ReadParameters();
			(uint revision, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) layout) ret3;
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<(uint revision, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]) layout)> tsc3 = new TaskCompletionSource<(uint, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[]))>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						(uint, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[])) result3 = await OnGetLayoutAsync(parentId, recursionDepth, propertyNames2);
						tsc3.SetResult(result3);
					}
					catch (Exception exception3)
					{
						tsc3.SetException(exception3);
					}
				}, null);
				(uint, (int, Dictionary<string, DBusVariantItem>, DBusVariantItem[])) tuple2 = await tsc3.Task;
				ret3 = tuple2;
			}
			else
			{
				ret3 = await OnGetLayoutAsync(parentId, recursionDepth, propertyNames2);
			}
			Reply();
			break;
		}
		case "GetGroupProperties":
		{
			if (!(memberAsString == "aias"))
			{
				break;
			}
			int[] ids2 = default(int[]);
			string[] propertyNames = default(string[]);
			ReadParameters();
			(int, Dictionary<string, DBusVariantItem>)[] ret2;
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<(int, Dictionary<string, DBusVariantItem>)[]> tsc2 = new TaskCompletionSource<(int, Dictionary<string, DBusVariantItem>)[]>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						(int, Dictionary<string, DBusVariantItem>)[] result2 = await OnGetGroupPropertiesAsync(ids2, propertyNames);
						tsc2.SetResult(result2);
					}
					catch (Exception exception2)
					{
						tsc2.SetException(exception2);
					}
				}, null);
				(int, Dictionary<string, DBusVariantItem>)[] array = await tsc2.Task;
				ret2 = array;
			}
			else
			{
				ret2 = await OnGetGroupPropertiesAsync(ids2, propertyNames);
			}
			Reply();
			break;
		}
		case "GetProperty":
		{
			if (!(memberAsString == "is"))
			{
				break;
			}
			int id = default(int);
			string name = default(string);
			ReadParameters();
			DBusVariantItem ret5;
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<DBusVariantItem> tsc5 = new TaskCompletionSource<DBusVariantItem>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						DBusVariantItem result5 = await OnGetPropertyAsync(id, name);
						tsc5.SetResult(result5);
					}
					catch (Exception exception5)
					{
						tsc5.SetException(exception5);
					}
				}, null);
				DBusVariantItem dBusVariantItem = await tsc5.Task;
				ret5 = dBusVariantItem;
			}
			else
			{
				ret5 = await OnGetPropertyAsync(id, name);
			}
			Reply();
			break;
		}
		case "Event":
		{
			if (!(memberAsString == "isvu"))
			{
				break;
			}
			int id3 = default(int);
			string eventId = default(string);
			DBusVariantItem data = default(DBusVariantItem);
			uint timestamp = default(uint);
			ReadParameters();
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<bool> tsc7 = new TaskCompletionSource<bool>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						await OnEventAsync(id3, eventId, data, timestamp);
						tsc7.SetResult(result: true);
					}
					catch (Exception exception7)
					{
						tsc7.SetException(exception7);
					}
				}, null);
				await tsc7.Task;
			}
			else
			{
				await OnEventAsync(id3, eventId, data, timestamp);
			}
			break;
		}
		case "EventGroup":
		{
			if (!(memberAsString == "a(isvu)"))
			{
				break;
			}
			(int, string, DBusVariantItem, uint)[] events = default((int, string, DBusVariantItem, uint)[]);
			ReadParameters();
			int[] ret4;
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<int[]> tsc4 = new TaskCompletionSource<int[]>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						int[] result4 = await OnEventGroupAsync(events);
						tsc4.SetResult(result4);
					}
					catch (Exception exception4)
					{
						tsc4.SetException(exception4);
					}
				}, null);
				int[] array2 = await tsc4.Task;
				ret4 = array2;
			}
			else
			{
				ret4 = await OnEventGroupAsync(events);
			}
			Reply();
			break;
		}
		case "AboutToShow":
		{
			if (!(memberAsString == "i"))
			{
				break;
			}
			int id2 = default(int);
			ReadParameters();
			bool ret6;
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<bool> tsc6 = new TaskCompletionSource<bool>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						bool result6 = await OnAboutToShowAsync(id2);
						tsc6.SetResult(result6);
					}
					catch (Exception exception6)
					{
						tsc6.SetException(exception6);
					}
				}, null);
				bool flag = await tsc6.Task;
				ret6 = flag;
			}
			else
			{
				ret6 = await OnAboutToShowAsync(id2);
			}
			Reply();
			break;
		}
		case "AboutToShowGroup":
		{
			if (!(memberAsString == "ai"))
			{
				break;
			}
			int[] ids = default(int[]);
			ReadParameters();
			(int[] updatesNeeded, int[] idErrors) ret;
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<(int[] updatesNeeded, int[] idErrors)> tsc = new TaskCompletionSource<(int[], int[])>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						(int[], int[]) result = await OnAboutToShowGroupAsync(ids);
						tsc.SetResult(result);
					}
					catch (Exception exception)
					{
						tsc.SetException(exception);
					}
				}, null);
				(int[], int[]) tuple = await tsc.Task;
				ret = tuple;
			}
			else
			{
				ret = await OnAboutToShowGroupAsync(ids);
			}
			Reply();
			break;
		}
		}
		void Reply()
		{
			MessageWriter writer = context.CreateReplyWriter("a{sv}");
			Dictionary<string, DBusVariantItem> values = new Dictionary<string, DBusVariantItem>
			{
				{
					"Version",
					new DBusVariantItem("u", new DBusUInt32Item(BackingProperties.Version))
				},
				{
					"TextDirection",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.TextDirection))
				},
				{
					"Status",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.Status))
				},
				{
					"IconThemePath",
					new DBusVariantItem("as", new DBusArrayItem(DBusType.String, BackingProperties.IconThemePath.Select((string x) => new DBusStringItem(x))))
				}
			};
			writer.WriteDictionary_aesv(values);
			context.Reply(writer.CreateMessage());
		}
		void Reply()
		{
			Reader bodyReader = context.Request.GetBodyReader();
			bodyReader.ReadString();
			switch (bodyReader.ReadString())
			{
			case "Version":
			{
				MessageWriter writer3 = context.CreateReplyWriter("v");
				writer3.WriteDBusVariant(new DBusVariantItem("u", new DBusUInt32Item(BackingProperties.Version)));
				context.Reply(writer3.CreateMessage());
				writer3.Dispose();
				break;
			}
			case "TextDirection":
			{
				MessageWriter writer4 = context.CreateReplyWriter("v");
				writer4.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.TextDirection)));
				context.Reply(writer4.CreateMessage());
				writer4.Dispose();
				break;
			}
			case "Status":
			{
				MessageWriter writer5 = context.CreateReplyWriter("v");
				writer5.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.Status)));
				context.Reply(writer5.CreateMessage());
				writer5.Dispose();
				break;
			}
			case "IconThemePath":
			{
				MessageWriter writer2 = context.CreateReplyWriter("v");
				writer2.WriteDBusVariant(new DBusVariantItem("as", new DBusArrayItem(DBusType.String, BackingProperties.IconThemePath.Select((string x) => new DBusStringItem(x)))));
				context.Reply(writer2.CreateMessage());
				writer2.Dispose();
				break;
			}
			}
		}
	}
}
