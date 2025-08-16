namespace TextMateSharp.Internal.Oniguruma;

public class OnigResult
{
	private int _indexInScanner;

	private Region _region;

	public OnigResult(Region region, int indexInScanner)
	{
		_region = region;
		_indexInScanner = indexInScanner;
	}

	public int GetIndex()
	{
		return _indexInScanner;
	}

	public void SetIndex(int index)
	{
		_indexInScanner = index;
	}

	public int LocationAt(int index)
	{
		int bytes = _region.Start[index];
		if (bytes > 0)
		{
			return bytes;
		}
		return 0;
	}

	public int Count()
	{
		return _region.NumRegs;
	}

	public int LengthAt(int index)
	{
		int bytes = _region.End[index] - _region.Start[index];
		if (bytes > 0)
		{
			return bytes;
		}
		return 0;
	}
}
