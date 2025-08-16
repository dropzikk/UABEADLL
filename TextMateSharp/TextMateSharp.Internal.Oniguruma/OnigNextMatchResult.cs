using System.Text;

namespace TextMateSharp.Internal.Oniguruma;

internal class OnigNextMatchResult : IOnigNextMatchResult
{
	private class OnigCaptureIndex : IOnigCaptureIndex
	{
		public int Index { get; private set; }

		public int Start { get; private set; }

		public int End { get; private set; }

		public int Length => End - Start;

		public OnigCaptureIndex(int index, int start, int end)
		{
			Index = index;
			Start = ((start >= 0) ? start : 0);
			End = ((end >= 0) ? end : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{\"index\": ");
			stringBuilder.Append(Index);
			stringBuilder.Append(", \"start\": ");
			stringBuilder.Append(Start);
			stringBuilder.Append(", \"end\": ");
			stringBuilder.Append(End);
			stringBuilder.Append(", \"length\": ");
			stringBuilder.Append(Length);
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}

	private int _index;

	private IOnigCaptureIndex[] _captureIndices;

	public OnigNextMatchResult(OnigResult result)
	{
		_index = result.GetIndex();
		_captureIndices = CaptureIndicesForMatch(result);
	}

	public int GetIndex()
	{
		return _index;
	}

	public IOnigCaptureIndex[] GetCaptureIndices()
	{
		return _captureIndices;
	}

	public override string ToString()
	{
		StringBuilder result = new StringBuilder();
		result.Append("{\n");
		result.Append("  \"index\": ");
		result.Append(GetIndex());
		result.Append(",\n");
		result.Append("  \"captureIndices\": [\n");
		int i = 0;
		IOnigCaptureIndex[] captureIndices = GetCaptureIndices();
		foreach (IOnigCaptureIndex captureIndex in captureIndices)
		{
			if (i > 0)
			{
				result.Append(",\n");
			}
			result.Append("    ");
			result.Append(captureIndex);
			i++;
		}
		result.Append("\n");
		result.Append("  ]\n");
		result.Append("}");
		return result.ToString();
	}

	private static IOnigCaptureIndex[] CaptureIndicesForMatch(OnigResult result)
	{
		int resultCount = result.Count();
		IOnigCaptureIndex[] captures = new IOnigCaptureIndex[resultCount];
		for (int index = 0; index < resultCount; index++)
		{
			int captureStart = result.LocationAt(index);
			int captureEnd = result.LocationAt(index) + result.LengthAt(index);
			captures[index] = new OnigCaptureIndex(index, captureStart, captureEnd);
		}
		return captures;
	}
}
