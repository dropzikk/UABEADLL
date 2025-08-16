using System;
using System.Collections.Generic;
using Avalonia.Metadata;

namespace Avalonia.Rendering;

[PrivateApi]
public interface IHitTester
{
	IEnumerable<Visual> HitTest(Point p, Visual root, Func<Visual, bool> filter);

	Visual? HitTestFirst(Point p, Visual root, Func<Visual, bool> filter);
}
