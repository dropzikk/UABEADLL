using System;
using System.ComponentModel;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Platform;

namespace Avalonia.Controls.Embedding;

public class EmbeddableControlRoot : TopLevel, IFocusScope, IDisposable
{
	protected bool EnforceClientSize { get; set; } = true;

	protected override Type StyleKeyOverride => typeof(EmbeddableControlRoot);

	public EmbeddableControlRoot(ITopLevelImpl impl)
		: base(impl)
	{
	}

	public EmbeddableControlRoot()
		: base(PlatformManager.CreateEmbeddableWindow())
	{
	}

	public void Prepare()
	{
		EnsureInitialized();
		ApplyTemplate();
		base.LayoutManager.ExecuteInitialLayoutPass();
	}

	public new void StartRendering()
	{
		base.StartRendering();
	}

	public new void StopRendering()
	{
		base.StopRendering();
	}

	private void EnsureInitialized()
	{
		if (!base.IsInitialized)
		{
			((ISupportInitialize)this).BeginInit();
			((ISupportInitialize)this).EndInit();
		}
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		if (EnforceClientSize)
		{
			availableSize = base.PlatformImpl?.ClientSize ?? default(Size);
		}
		Size result = base.MeasureOverride(availableSize);
		if (EnforceClientSize)
		{
			return availableSize;
		}
		return result;
	}

	public void Dispose()
	{
		base.PlatformImpl?.Dispose();
	}
}
