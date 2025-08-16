using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Avalonia.Diagnostics;

internal class ClassicDesktopStyleApplicationLifetimeTopLevelGroup : IDevToolsTopLevelGroup
{
	private readonly IClassicDesktopStyleApplicationLifetime _lifetime;

	public IReadOnlyList<TopLevel> Items => _lifetime.Windows;

	public ClassicDesktopStyleApplicationLifetimeTopLevelGroup(IClassicDesktopStyleApplicationLifetime lifetime)
	{
		_lifetime = lifetime ?? throw new ArgumentNullException("lifetime");
	}

	public override int GetHashCode()
	{
		return _lifetime.GetHashCode();
	}

	public override bool Equals(object? obj)
	{
		if (obj is ClassicDesktopStyleApplicationLifetimeTopLevelGroup classicDesktopStyleApplicationLifetimeTopLevelGroup)
		{
			return classicDesktopStyleApplicationLifetimeTopLevelGroup._lifetime == _lifetime;
		}
		return false;
	}
}
