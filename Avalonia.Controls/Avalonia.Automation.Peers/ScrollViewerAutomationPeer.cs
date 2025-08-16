using System;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Utilities;

namespace Avalonia.Automation.Peers;

public class ScrollViewerAutomationPeer : ControlAutomationPeer, IScrollProvider
{
	public new ScrollViewer Owner => (ScrollViewer)base.Owner;

	public bool HorizontallyScrollable => MathUtilities.GreaterThan(Owner.Extent.Width, Owner.Viewport.Width);

	public double HorizontalScrollPercent
	{
		get
		{
			if (!HorizontallyScrollable)
			{
				return -1.0;
			}
			return Owner.Offset.X * 100.0 / (Owner.Extent.Width - Owner.Viewport.Width);
		}
	}

	public double HorizontalViewSize
	{
		get
		{
			if (MathUtilities.IsZero(Owner.Extent.Width))
			{
				return 100.0;
			}
			return Math.Min(100.0, Owner.Viewport.Width * 100.0 / Owner.Extent.Width);
		}
	}

	public bool VerticallyScrollable => MathUtilities.GreaterThan(Owner.Extent.Height, Owner.Viewport.Height);

	public double VerticalScrollPercent
	{
		get
		{
			if (!VerticallyScrollable)
			{
				return -1.0;
			}
			return Owner.Offset.Y * 100.0 / (Owner.Extent.Height - Owner.Viewport.Height);
		}
	}

	public double VerticalViewSize
	{
		get
		{
			if (MathUtilities.IsZero(Owner.Extent.Height))
			{
				return 100.0;
			}
			return Math.Min(100.0, Owner.Viewport.Height * 100.0 / Owner.Extent.Height);
		}
	}

	public ScrollViewerAutomationPeer(ScrollViewer owner)
		: base(owner)
	{
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Pane;
	}

	protected override bool IsContentElementCore()
	{
		return false;
	}

	protected override bool IsControlElementCore()
	{
		if (Owner.TemplatedParent == null)
		{
			return base.IsControlElementCore();
		}
		return false;
	}

	public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
	{
		if (!IsEnabled())
		{
			throw new ElementNotEnabledException();
		}
		bool num = horizontalAmount != ScrollAmount.NoAmount;
		bool flag = verticalAmount != ScrollAmount.NoAmount;
		if ((num && !HorizontallyScrollable) || (flag && !VerticallyScrollable))
		{
			throw new InvalidOperationException("Operation cannot be performed");
		}
		switch (horizontalAmount)
		{
		case ScrollAmount.LargeDecrement:
			Owner.PageLeft();
			break;
		case ScrollAmount.SmallDecrement:
			Owner.LineLeft();
			break;
		case ScrollAmount.SmallIncrement:
			Owner.LineRight();
			break;
		case ScrollAmount.LargeIncrement:
			Owner.PageRight();
			break;
		default:
			throw new InvalidOperationException("Operation cannot be performed");
		case ScrollAmount.NoAmount:
			break;
		}
		switch (verticalAmount)
		{
		case ScrollAmount.LargeDecrement:
			Owner.PageUp();
			break;
		case ScrollAmount.SmallDecrement:
			Owner.LineUp();
			break;
		case ScrollAmount.SmallIncrement:
			Owner.LineDown();
			break;
		case ScrollAmount.LargeIncrement:
			Owner.PageDown();
			break;
		default:
			throw new InvalidOperationException("Operation cannot be performed");
		case ScrollAmount.NoAmount:
			break;
		}
	}

	public void SetScrollPercent(double horizontalPercent, double verticalPercent)
	{
		if (!IsEnabled())
		{
			throw new ElementNotEnabledException();
		}
		bool num = horizontalPercent != -1.0;
		bool flag = verticalPercent != -1.0;
		if ((num && !HorizontallyScrollable) || (flag && !VerticallyScrollable))
		{
			throw new InvalidOperationException("Operation cannot be performed");
		}
		if ((num && horizontalPercent < 0.0) || horizontalPercent > 100.0)
		{
			throw new ArgumentOutOfRangeException("horizontalPercent");
		}
		if ((flag && verticalPercent < 0.0) || verticalPercent > 100.0)
		{
			throw new ArgumentOutOfRangeException("verticalPercent");
		}
		Vector offset = Owner.Offset;
		if (num)
		{
			offset = offset.WithX((Owner.Extent.Width - Owner.Viewport.Width) * horizontalPercent * 0.01);
		}
		if (flag)
		{
			offset = offset.WithY((Owner.Extent.Height - Owner.Viewport.Height) * verticalPercent * 0.01);
		}
		Owner.Offset = offset;
	}
}
