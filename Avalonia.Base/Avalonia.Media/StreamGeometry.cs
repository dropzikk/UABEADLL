using Avalonia.Platform;

namespace Avalonia.Media;

public class StreamGeometry : Geometry
{
	private IStreamGeometryImpl? _impl;

	public StreamGeometry()
	{
	}

	private StreamGeometry(IStreamGeometryImpl impl)
	{
		_impl = impl;
	}

	public new static StreamGeometry Parse(string s)
	{
		StreamGeometry streamGeometry = new StreamGeometry();
		using StreamGeometryContext geometryContext = streamGeometry.Open();
		using PathMarkupParser pathMarkupParser = new PathMarkupParser(geometryContext);
		pathMarkupParser.Parse(s);
		return streamGeometry;
	}

	public override Geometry Clone()
	{
		return new StreamGeometry(((IStreamGeometryImpl)base.PlatformImpl).Clone());
	}

	public StreamGeometryContext Open()
	{
		return new StreamGeometryContext(((IStreamGeometryImpl)base.PlatformImpl).Open());
	}

	private protected override IGeometryImpl? CreateDefiningGeometry()
	{
		if (_impl == null)
		{
			IPlatformRenderInterface requiredService = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
			_impl = requiredService.CreateStreamGeometry();
		}
		return _impl;
	}
}
