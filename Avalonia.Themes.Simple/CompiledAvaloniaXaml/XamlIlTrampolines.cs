using Avalonia.Controls;
using Avalonia.Dialogs.Internal;

namespace CompiledAvaloniaXaml;

internal class XamlIlTrampolines
{
	public static void Avalonia_002EControls_003AAvalonia_002EControls_002ETextBox_002BCut_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TextBox)P_0).Cut();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002ETextBox_002BCopy_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TextBox)P_0).Copy();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002ETextBox_002BPaste_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TextBox)P_0).Paste();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002ETextBox_002BClear_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TextBox)P_0).Clear();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002EScrollViewer_002BLineUp_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ScrollViewer)P_0).LineUp();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002EScrollViewer_002BLineDown_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ScrollViewer)P_0).LineDown();
	}

	public static void Avalonia_002EDialogs_003AAvalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002BGoUp_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).GoUp();
	}

	public static void Avalonia_002EDialogs_003AAvalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002BRefresh_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).Refresh();
	}

	public static void Avalonia_002EDialogs_003AAvalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002BEnterPressed_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).EnterPressed();
	}

	public static void Avalonia_002EDialogs_003AAvalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002BOk_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).Ok();
	}

	public static void Avalonia_002EDialogs_003AAvalonia_002EDialogs_002EInternal_002EManagedFileChooserViewModel_002BCancel_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ManagedFileChooserViewModel)P_0).Cancel();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002ESelectableTextBlock_002BCopy_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((SelectableTextBlock)P_0).Copy();
	}
}
