using System;
using System.ComponentModel;

namespace Avalonia.Controls.Embedding.Offscreen;

internal class OffscreenTopLevel : TopLevel
{
	public OffscreenTopLevelImplBase Impl { get; }

	protected override Type StyleKeyOverride => typeof(EmbeddableControlRoot);

	public OffscreenTopLevel(OffscreenTopLevelImplBase impl)
		: base(impl)
	{
		Impl = impl;
		Prepare();
	}

	public void Prepare()
	{
		EnsureInitialized();
		ApplyTemplate();
		base.LayoutManager.ExecuteInitialLayoutPass();
	}

	private void EnsureInitialized()
	{
		if (!base.IsInitialized)
		{
			((ISupportInitialize)this).BeginInit();
			((ISupportInitialize)this).EndInit();
		}
	}

	public void Dispose()
	{
		base.PlatformImpl?.Dispose();
	}
}
