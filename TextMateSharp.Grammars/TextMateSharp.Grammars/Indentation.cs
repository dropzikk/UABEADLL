namespace TextMateSharp.Grammars;

public class Indentation
{
	public string Increase { get; set; } = string.Empty;

	public string Decrease { get; set; } = string.Empty;

	public string Unindent { get; set; } = string.Empty;

	public bool IsEmpty
	{
		get
		{
			if (!string.IsNullOrEmpty(Increase))
			{
				return string.IsNullOrEmpty(Decrease);
			}
			return true;
		}
	}
}
