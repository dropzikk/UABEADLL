using System;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;

namespace Avalonia.FreeDesktop;

internal class StatusNotifierItemDbusObj : OrgKdeStatusNotifierItem
{
	protected override Connection Connection { get; }

	public override string Path => "/StatusNotifierItem";

	public event Action? ActivationDelegate;

	public StatusNotifierItemDbusObj(Connection connection, ObjectPath dbusMenuPath)
	{
		Connection = connection;
		base.BackingProperties.Menu = dbusMenuPath;
		base.BackingProperties.Category = string.Empty;
		base.BackingProperties.Status = string.Empty;
		base.BackingProperties.Id = string.Empty;
		base.BackingProperties.Title = string.Empty;
		base.BackingProperties.IconPixmap = Array.Empty<(int, int, byte[])>();
		base.BackingProperties.AttentionIconName = string.Empty;
		base.BackingProperties.AttentionIconPixmap = Array.Empty<(int, int, byte[])>();
		base.BackingProperties.AttentionMovieName = string.Empty;
		base.BackingProperties.OverlayIconName = string.Empty;
		base.BackingProperties.OverlayIconPixmap = Array.Empty<(int, int, byte[])>();
		base.BackingProperties.ToolTip = (string.Empty, Array.Empty<(int, int, byte[])>(), string.Empty, string.Empty);
		InvalidateAll();
	}

	protected override ValueTask OnContextMenuAsync(int x, int y)
	{
		return default(ValueTask);
	}

	protected override ValueTask OnActivateAsync(int x, int y)
	{
		this.ActivationDelegate?.Invoke();
		return default(ValueTask);
	}

	protected override ValueTask OnSecondaryActivateAsync(int x, int y)
	{
		return default(ValueTask);
	}

	protected override ValueTask OnScrollAsync(int delta, string orientation)
	{
		return default(ValueTask);
	}

	public void InvalidateAll()
	{
		EmitNewTitle();
		EmitNewIcon();
		EmitNewAttentionIcon();
		EmitNewOverlayIcon();
		EmitNewToolTip();
		EmitNewStatus(base.BackingProperties.Status);
	}

	public void SetIcon((int, int, byte[]) dbusPixmap)
	{
		base.BackingProperties.IconPixmap = new(int, int, byte[])[1] { dbusPixmap };
		InvalidateAll();
	}

	public void SetTitleAndTooltip(string? text)
	{
		if (text != null)
		{
			base.BackingProperties.Id = text;
			base.BackingProperties.Category = "ApplicationStatus";
			base.BackingProperties.Status = text;
			base.BackingProperties.Title = text;
			InvalidateAll();
		}
	}
}
