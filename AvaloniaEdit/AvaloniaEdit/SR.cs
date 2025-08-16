using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AvaloniaEdit;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class SR
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("AvaloniaEdit.SR", typeof(SR).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string ReplaceLabel => ResourceManager.GetString("ReplaceLabel", resourceCulture);

	public static string Search1Match => ResourceManager.GetString("Search1Match", resourceCulture);

	public static string SearchErrorText => ResourceManager.GetString("SearchErrorText", resourceCulture);

	public static string SearchFindNextText => ResourceManager.GetString("SearchFindNextText", resourceCulture);

	public static string SearchFindPreviousText => ResourceManager.GetString("SearchFindPreviousText", resourceCulture);

	public static string SearchLabel => ResourceManager.GetString("SearchLabel", resourceCulture);

	public static string SearchMatchCaseText => ResourceManager.GetString("SearchMatchCaseText", resourceCulture);

	public static string SearchMatchWholeWordsText => ResourceManager.GetString("SearchMatchWholeWordsText", resourceCulture);

	public static string SearchNoMatchesFoundText => ResourceManager.GetString("SearchNoMatchesFoundText", resourceCulture);

	public static string SearchReplaceAll => ResourceManager.GetString("SearchReplaceAll", resourceCulture);

	public static string SearchReplaceNext => ResourceManager.GetString("SearchReplaceNext", resourceCulture);

	public static string SearchToggleReplace => ResourceManager.GetString("SearchToggleReplace", resourceCulture);

	public static string SearchUseRegexText => ResourceManager.GetString("SearchUseRegexText", resourceCulture);

	public static string SearchXMatches => ResourceManager.GetString("SearchXMatches", resourceCulture);

	public static string SearchXOfY => ResourceManager.GetString("SearchXOfY", resourceCulture);

	internal SR()
	{
	}
}
