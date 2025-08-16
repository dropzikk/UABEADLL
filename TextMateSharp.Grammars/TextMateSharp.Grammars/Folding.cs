using System.Text.Json.Serialization;

namespace TextMateSharp.Grammars;

public class Folding
{
	[JsonPropertyName("offSide")]
	public bool OffSide { get; set; }

	[JsonPropertyName("markers")]
	public Markers Markers { get; set; }

	public bool IsEmpty
	{
		get
		{
			if (Markers != null && !string.IsNullOrEmpty(Markers.Start))
			{
				return string.IsNullOrEmpty(Markers.End);
			}
			return true;
		}
	}
}
