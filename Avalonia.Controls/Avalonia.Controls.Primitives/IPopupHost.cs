using System;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Controls.Primitives;

[NotClientImplementable]
public interface IPopupHost : IDisposable, IFocusScope
{
	double Width { get; set; }

	double MinWidth { get; set; }

	double MaxWidth { get; set; }

	double Height { get; set; }

	double MinHeight { get; set; }

	double MaxHeight { get; set; }

	ContentPresenter? Presenter { get; }

	bool Topmost { get; set; }

	Transform? Transform { get; set; }

	Visual? HostedVisualTreeRoot { get; }

	event EventHandler<TemplateAppliedEventArgs>? TemplateApplied;

	void ConfigurePosition(Visual target, PlacementMode placement, Point offset, PopupAnchor anchor = PopupAnchor.None, PopupGravity gravity = PopupGravity.None, PopupPositionerConstraintAdjustment constraintAdjustment = PopupPositionerConstraintAdjustment.All, Rect? rect = null);

	void SetChild(Control? control);

	void Show();

	void Hide();
}
