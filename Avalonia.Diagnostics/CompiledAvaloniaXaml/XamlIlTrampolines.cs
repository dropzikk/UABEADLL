using Avalonia.Controls;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Diagnostics.Views;

namespace CompiledAvaloniaXaml;

internal class XamlIlTrampolines
{
	public static void Avalonia_002EControls_003AAvalonia_002EControls_002ETextBox_002BClear_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TextBox)P_0).Clear();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EControlDetailsViewModel_002BNavigateToParentProperty_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ControlDetailsViewModel)P_0).NavigateToParentProperty();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyPropertyName_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((SetterViewModel)P_0).CopyPropertyName();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EBindingSetterViewModel_002BCopyValue_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((BindingSetterViewModel)P_0).CopyValue();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ESetterViewModel_002BCopyValue_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((SetterViewModel)P_0).CopyValue();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EResourceSetterViewModel_002BCopyResourceKey_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((ResourceSetterViewModel)P_0).CopyResourceKey();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002BDisableAll_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((EventsPageViewModel)P_0).DisableAll();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002BEnableDefault_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((EventsPageViewModel)P_0).EnableDefault();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EEventsPageViewModel_002BClear_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((EventsPageViewModel)P_0).Clear();
	}

	public static void Avalonia_002EControls_003AAvalonia_002EControls_002EWindow_002BClose_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((Window)P_0).Close();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BShot_1_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).Shot(P_1);
	}

	public static bool Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BCanShot_0021CommandCanExecuteTrampoline(object P_0, object P_1)
	{
		return ((MainViewModel)P_0).CanShot(P_1);
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViews_002EMainView_002BToggleConsole_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainView)P_0).ToggleConsole();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleShowImplementedInterfaces_1_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleShowImplementedInterfaces(P_1);
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleShowDetailsPropertyType_1_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleShowDetailsPropertyType(P_1);
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleVisualizeMarginPadding_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleVisualizeMarginPadding();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleDirtyRectsOverlay_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleDirtyRectsOverlay();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleFpsOverlay_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleFpsOverlay();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleLayoutTimeGraphOverlay_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleLayoutTimeGraphOverlay();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002EMainViewModel_002BToggleRenderTimeGraphOverlay_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((MainViewModel)P_0).ToggleRenderTimeGraphOverlay();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCopySelector_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).CopySelector();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCopySelectorFromTemplateParent_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).CopySelectorFromTemplateParent();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BExpandRecursively_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).ExpandRecursively();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCollapseChildren_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).CollapseChildren();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BCaptureNodeScreenshot_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).CaptureNodeScreenshot();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BBringIntoView_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).BringIntoView();
	}

	public static void Avalonia_002EDiagnostics_003AAvalonia_002EDiagnostics_002EViewModels_002ETreePageViewModel_002BFocus_0_0021CommandExecuteTrampoline(object P_0, object P_1)
	{
		((TreePageViewModel)P_0).Focus();
	}
}
