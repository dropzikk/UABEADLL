using System;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls.Primitives;

namespace Avalonia.Controls.Automation.Peers;

public class ProgressBarAutomationPeer : RangeBaseAutomationPeer, IRangeValueProvider
{
	bool IRangeValueProvider.IsReadOnly => true;

	double IRangeValueProvider.LargeChange => double.NaN;

	double IRangeValueProvider.SmallChange => double.NaN;

	public ProgressBarAutomationPeer(RangeBase owner)
		: base(owner)
	{
	}

	protected override string GetClassNameCore()
	{
		return "ProgressBar";
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.ProgressBar;
	}

	void IRangeValueProvider.SetValue(double val)
	{
		throw new InvalidOperationException("ProgressBar is ReadOnly, value can't be set.");
	}
}
