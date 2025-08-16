using System;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop;

internal interface IAvnMacOptions : IUnknown, IDisposable
{
	void SetShowInDock(int show);

	void SetApplicationTitle(string utf8string);

	void SetDisableSetProcessName(int disable);

	void SetDisableAppDelegate(int disable);
}
