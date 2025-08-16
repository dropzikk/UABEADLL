using System;
using System.ComponentModel;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021XamlLoader
{
	public static object TryLoad(IServiceProvider P_0, string P_1)
	{
		if (string.Equals(P_1, "avares://AvaloniaEdit/CodeCompletion/CompletionList.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FCodeCompletion_002FCompletionList_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/CodeCompletion/CompletionWindow.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FCodeCompletion_002FCompletionWindow_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/CodeCompletion/InsightWindow.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FCodeCompletion_002FInsightWindow_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Editing/TextArea.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FEditing_002FTextArea_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Rendering/VisualLineDrawingVisual.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FRendering_002FVisualLineDrawingVisual_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Search/SearchPanel.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FSearch_002FSearchPanel_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/TextEditor.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FTextEditor_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FThemes_002FFluent_002FAvaloniaEdit_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Themes/Fluent/Base.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FThemes_002FFluent_002FBase_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Themes/Simple/AvaloniaEdit.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FThemes_002FSimple_002FAvaloniaEdit_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		if (string.Equals(P_1, "avares://AvaloniaEdit/Themes/Simple/Base.xaml", StringComparison.OrdinalIgnoreCase))
		{
			return _0021AvaloniaResources.Build_003A_002FThemes_002FSimple_002FBase_002Examl(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(P_0));
		}
		return null;
	}

	public static object TryLoad(string P_0)
	{
		return TryLoad(null, P_0);
	}
}
