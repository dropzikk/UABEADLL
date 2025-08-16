namespace Avalonia.Media.TextFormatting;

public readonly struct SplitResult<T>
{
	public T First { get; }

	public T? Second { get; }

	public SplitResult(T first, T? second)
	{
		First = first;
		Second = second;
	}

	public void Deconstruct(out T first, out T? second)
	{
		first = First;
		second = Second;
	}
}
