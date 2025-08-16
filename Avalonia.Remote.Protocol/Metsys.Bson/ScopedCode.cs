namespace Metsys.Bson;

internal class ScopedCode
{
	public string CodeString { get; set; }

	public object Scope { get; set; }
}
internal class ScopedCode<T> : ScopedCode
{
	public new T Scope { get; set; }
}
