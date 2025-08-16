using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal abstract class OrgKdeStatusNotifierItem : IMethodHandler
{
	public class Properties
	{
		public string Category { get; set; }

		public string Id { get; set; }

		public string Title { get; set; }

		public string Status { get; set; }

		public int WindowId { get; set; }

		public string IconThemePath { get; set; }

		public ObjectPath Menu { get; set; }

		public bool ItemIsMenu { get; set; }

		public string IconName { get; set; }

		public (int, int, byte[])[] IconPixmap { get; set; }

		public string OverlayIconName { get; set; }

		public (int, int, byte[])[] OverlayIconPixmap { get; set; }

		public string AttentionIconName { get; set; }

		public (int, int, byte[])[] AttentionIconPixmap { get; set; }

		public string AttentionMovieName { get; set; }

		public (string, (int, int, byte[])[], string, string) ToolTip { get; set; }
	}

	private SynchronizationContext? _synchronizationContext;

	protected abstract Connection Connection { get; }

	public abstract string Path { get; }

	public Properties BackingProperties { get; } = new Properties();

	public OrgKdeStatusNotifierItem(bool emitOnCapturedContext = true)
	{
		if (emitOnCapturedContext)
		{
			_synchronizationContext = SynchronizationContext.Current;
		}
	}

	protected abstract ValueTask OnContextMenuAsync(int x, int y);

	protected abstract ValueTask OnActivateAsync(int x, int y);

	protected abstract ValueTask OnSecondaryActivateAsync(int x, int y);

	protected abstract ValueTask OnScrollAsync(int delta, string orientation);

	protected void EmitNewTitle()
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "org.kde.StatusNotifierItem", "NewTitle");
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	protected void EmitNewIcon()
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "org.kde.StatusNotifierItem", "NewIcon");
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	protected void EmitNewAttentionIcon()
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "org.kde.StatusNotifierItem", "NewAttentionIcon");
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	protected void EmitNewOverlayIcon()
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "org.kde.StatusNotifierItem", "NewOverlayIcon");
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	protected void EmitNewToolTip()
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "org.kde.StatusNotifierItem", "NewToolTip");
		Connection.TrySendMessage(messageWriter.CreateMessage());
		messageWriter.Dispose();
	}

	protected void EmitNewStatus(string status)
	{
		MessageWriter messageWriter = Connection.GetMessageWriter();
		messageWriter.WriteSignalHeader(null, Path, "org.kde.StatusNotifierItem", "NewStatus", "s");
		messageWriter.WriteString(status);
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
		if (!(interfaceAsString == "org.kde.StatusNotifierItem"))
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
		case "ContextMenu":
		{
			if (!(memberAsString == "ii"))
			{
				break;
			}
			int x3 = default(int);
			int y2 = default(int);
			ReadParameters();
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<bool> tsc3 = new TaskCompletionSource<bool>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						await OnContextMenuAsync(x3, y2);
						tsc3.SetResult(result: true);
					}
					catch (Exception exception3)
					{
						tsc3.SetException(exception3);
					}
				}, null);
				await tsc3.Task;
			}
			else
			{
				await OnContextMenuAsync(x3, y2);
			}
			break;
		}
		case "Activate":
		{
			if (!(memberAsString == "ii"))
			{
				break;
			}
			int x4 = default(int);
			int y3 = default(int);
			ReadParameters();
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<bool> tsc4 = new TaskCompletionSource<bool>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						await OnActivateAsync(x4, y3);
						tsc4.SetResult(result: true);
					}
					catch (Exception exception4)
					{
						tsc4.SetException(exception4);
					}
				}, null);
				await tsc4.Task;
			}
			else
			{
				await OnActivateAsync(x4, y3);
			}
			break;
		}
		case "SecondaryActivate":
		{
			if (!(memberAsString == "ii"))
			{
				break;
			}
			int x2 = default(int);
			int y = default(int);
			ReadParameters();
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<bool> tsc2 = new TaskCompletionSource<bool>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						await OnSecondaryActivateAsync(x2, y);
						tsc2.SetResult(result: true);
					}
					catch (Exception exception2)
					{
						tsc2.SetException(exception2);
					}
				}, null);
				await tsc2.Task;
			}
			else
			{
				await OnSecondaryActivateAsync(x2, y);
			}
			break;
		}
		case "Scroll":
		{
			if (!(memberAsString == "is"))
			{
				break;
			}
			int delta = default(int);
			string orientation = default(string);
			ReadParameters();
			if (_synchronizationContext != null)
			{
				TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
				_synchronizationContext.Post(async delegate
				{
					try
					{
						await OnScrollAsync(delta, orientation);
						tsc.SetResult(result: true);
					}
					catch (Exception exception)
					{
						tsc.SetException(exception);
					}
				}, null);
				await tsc.Task;
			}
			else
			{
				await OnScrollAsync(delta, orientation);
			}
			break;
		}
		}
		void Reply()
		{
			MessageWriter writer = context.CreateReplyWriter("a{sv}");
			Dictionary<string, DBusVariantItem> values = new Dictionary<string, DBusVariantItem>
			{
				{
					"Category",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.Category))
				},
				{
					"Id",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.Id))
				},
				{
					"Title",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.Title))
				},
				{
					"Status",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.Status))
				},
				{
					"WindowId",
					new DBusVariantItem("i", new DBusInt32Item(BackingProperties.WindowId))
				},
				{
					"IconThemePath",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.IconThemePath))
				},
				{
					"Menu",
					new DBusVariantItem("o", new DBusObjectPathItem(BackingProperties.Menu))
				},
				{
					"ItemIsMenu",
					new DBusVariantItem("b", new DBusBoolItem(BackingProperties.ItemIsMenu))
				},
				{
					"IconName",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.IconName))
				},
				{
					"IconPixmap",
					new DBusVariantItem("a(iiay)", new DBusArrayItem(DBusType.Struct, BackingProperties.IconPixmap.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
					{
						new DBusInt32Item(x.Item1),
						new DBusInt32Item(x.Item2),
						new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
					}))))
				},
				{
					"OverlayIconName",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.OverlayIconName))
				},
				{
					"OverlayIconPixmap",
					new DBusVariantItem("a(iiay)", new DBusArrayItem(DBusType.Struct, BackingProperties.OverlayIconPixmap.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
					{
						new DBusInt32Item(x.Item1),
						new DBusInt32Item(x.Item2),
						new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
					}))))
				},
				{
					"AttentionIconName",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.AttentionIconName))
				},
				{
					"AttentionIconPixmap",
					new DBusVariantItem("a(iiay)", new DBusArrayItem(DBusType.Struct, BackingProperties.AttentionIconPixmap.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
					{
						new DBusInt32Item(x.Item1),
						new DBusInt32Item(x.Item2),
						new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
					}))))
				},
				{
					"AttentionMovieName",
					new DBusVariantItem("s", new DBusStringItem(BackingProperties.AttentionMovieName))
				},
				{
					"ToolTip",
					new DBusVariantItem("(sa(iiay)ss)", new DBusStructItem(new DBusItem[4]
					{
						new DBusStringItem(BackingProperties.ToolTip.Item1),
						new DBusArrayItem(DBusType.Struct, BackingProperties.ToolTip.Item2.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
						{
							new DBusInt32Item(x.Item1),
							new DBusInt32Item(x.Item2),
							new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
						}))),
						new DBusStringItem(BackingProperties.ToolTip.Item3),
						new DBusStringItem(BackingProperties.ToolTip.Item4)
					}))
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
			case "Category":
			{
				MessageWriter writer15 = context.CreateReplyWriter("v");
				writer15.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.Category)));
				context.Reply(writer15.CreateMessage());
				writer15.Dispose();
				break;
			}
			case "Id":
			{
				MessageWriter writer16 = context.CreateReplyWriter("v");
				writer16.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.Id)));
				context.Reply(writer16.CreateMessage());
				writer16.Dispose();
				break;
			}
			case "Title":
			{
				MessageWriter writer11 = context.CreateReplyWriter("v");
				writer11.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.Title)));
				context.Reply(writer11.CreateMessage());
				writer11.Dispose();
				break;
			}
			case "Status":
			{
				MessageWriter writer12 = context.CreateReplyWriter("v");
				writer12.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.Status)));
				context.Reply(writer12.CreateMessage());
				writer12.Dispose();
				break;
			}
			case "WindowId":
			{
				MessageWriter writer7 = context.CreateReplyWriter("v");
				writer7.WriteDBusVariant(new DBusVariantItem("i", new DBusInt32Item(BackingProperties.WindowId)));
				context.Reply(writer7.CreateMessage());
				writer7.Dispose();
				break;
			}
			case "IconThemePath":
			{
				MessageWriter writer8 = context.CreateReplyWriter("v");
				writer8.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.IconThemePath)));
				context.Reply(writer8.CreateMessage());
				writer8.Dispose();
				break;
			}
			case "Menu":
			{
				MessageWriter writer3 = context.CreateReplyWriter("v");
				writer3.WriteDBusVariant(new DBusVariantItem("o", new DBusObjectPathItem(BackingProperties.Menu)));
				context.Reply(writer3.CreateMessage());
				writer3.Dispose();
				break;
			}
			case "ItemIsMenu":
			{
				MessageWriter writer4 = context.CreateReplyWriter("v");
				writer4.WriteDBusVariant(new DBusVariantItem("b", new DBusBoolItem(BackingProperties.ItemIsMenu)));
				context.Reply(writer4.CreateMessage());
				writer4.Dispose();
				break;
			}
			case "IconName":
			{
				MessageWriter writer17 = context.CreateReplyWriter("v");
				writer17.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.IconName)));
				context.Reply(writer17.CreateMessage());
				writer17.Dispose();
				break;
			}
			case "IconPixmap":
			{
				MessageWriter writer14 = context.CreateReplyWriter("v");
				writer14.WriteDBusVariant(new DBusVariantItem("a(iiay)", new DBusArrayItem(DBusType.Struct, BackingProperties.IconPixmap.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
				{
					new DBusInt32Item(x.Item1),
					new DBusInt32Item(x.Item2),
					new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
				})))));
				context.Reply(writer14.CreateMessage());
				writer14.Dispose();
				break;
			}
			case "OverlayIconName":
			{
				MessageWriter writer13 = context.CreateReplyWriter("v");
				writer13.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.OverlayIconName)));
				context.Reply(writer13.CreateMessage());
				writer13.Dispose();
				break;
			}
			case "OverlayIconPixmap":
			{
				MessageWriter writer10 = context.CreateReplyWriter("v");
				writer10.WriteDBusVariant(new DBusVariantItem("a(iiay)", new DBusArrayItem(DBusType.Struct, BackingProperties.OverlayIconPixmap.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
				{
					new DBusInt32Item(x.Item1),
					new DBusInt32Item(x.Item2),
					new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
				})))));
				context.Reply(writer10.CreateMessage());
				writer10.Dispose();
				break;
			}
			case "AttentionIconName":
			{
				MessageWriter writer9 = context.CreateReplyWriter("v");
				writer9.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.AttentionIconName)));
				context.Reply(writer9.CreateMessage());
				writer9.Dispose();
				break;
			}
			case "AttentionIconPixmap":
			{
				MessageWriter writer6 = context.CreateReplyWriter("v");
				writer6.WriteDBusVariant(new DBusVariantItem("a(iiay)", new DBusArrayItem(DBusType.Struct, BackingProperties.AttentionIconPixmap.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
				{
					new DBusInt32Item(x.Item1),
					new DBusInt32Item(x.Item2),
					new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
				})))));
				context.Reply(writer6.CreateMessage());
				writer6.Dispose();
				break;
			}
			case "AttentionMovieName":
			{
				MessageWriter writer5 = context.CreateReplyWriter("v");
				writer5.WriteDBusVariant(new DBusVariantItem("s", new DBusStringItem(BackingProperties.AttentionMovieName)));
				context.Reply(writer5.CreateMessage());
				writer5.Dispose();
				break;
			}
			case "ToolTip":
			{
				MessageWriter writer2 = context.CreateReplyWriter("v");
				writer2.WriteDBusVariant(new DBusVariantItem("(sa(iiay)ss)", new DBusStructItem(new DBusItem[4]
				{
					new DBusStringItem(BackingProperties.ToolTip.Item1),
					new DBusArrayItem(DBusType.Struct, BackingProperties.ToolTip.Item2.Select(((int, int, byte[]) x) => new DBusStructItem(new DBusItem[3]
					{
						new DBusInt32Item(x.Item1),
						new DBusInt32Item(x.Item2),
						new DBusArrayItem(DBusType.Byte, x.Item3.Select((byte x) => new DBusByteItem(x)))
					}))),
					new DBusStringItem(BackingProperties.ToolTip.Item3),
					new DBusStringItem(BackingProperties.ToolTip.Item4)
				})));
				context.Reply(writer2.CreateMessage());
				writer2.Dispose();
				break;
			}
			}
		}
	}
}
