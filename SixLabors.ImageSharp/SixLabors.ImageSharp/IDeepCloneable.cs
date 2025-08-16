namespace SixLabors.ImageSharp;

public interface IDeepCloneable<out T> where T : class
{
	T DeepClone();
}
public interface IDeepCloneable
{
	IDeepCloneable DeepClone();
}
