using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

namespace CompiledAvaloniaXaml;

[EditorBrowsable(EditorBrowsableState.Never)]
public class _0021AvaloniaResources
{
	public class NamespaceInfo_003A_002FAboutAvaloniaDialog_002Examl : IAvaloniaXamlIlXmlNamespaceInfoProvider
	{
		private IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> _xmlNamespaces;

		public static IAvaloniaXamlIlXmlNamespaceInfoProvider Singleton;

		public virtual IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> XmlNamespaces
		{
			get
			{
				if (_xmlNamespaces == null)
				{
					_xmlNamespaces = CreateNamespaces();
				}
				return _xmlNamespaces;
			}
		}

		static AvaloniaXamlIlXmlNamespaceInfo CreateNamespaceInfo(string P_0, string P_1)
		{
			return new AvaloniaXamlIlXmlNamespaceInfo
			{
				ClrNamespace = P_0,
				ClrAssemblyName = P_1
			};
		}

		static IReadOnlyDictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> CreateNamespaces()
		{
			Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>> dictionary = new Dictionary<string, IReadOnlyList<AvaloniaXamlIlXmlNamespaceInfo>>(3);
			dictionary.Add("", new AvaloniaXamlIlXmlNamespaceInfo[31]
			{
				CreateNamespaceInfo("Avalonia", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Animation", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Animation.Easings", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Data", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Data.Converters", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input.GestureRecognizers", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Input.TextInput", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Layout", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.LogicalTree", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media.Imaging", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Media.Transformation", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia.Styling", "Avalonia.Base"),
				CreateNamespaceInfo("Avalonia", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Automation", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Embedding", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Presenters", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Primitives", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Shapes", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Templates", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Notifications", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Chrome", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Controls.Documents", "Avalonia.Controls"),
				CreateNamespaceInfo("Avalonia.Data", "Avalonia.Markup"),
				CreateNamespaceInfo("Avalonia.Markup.Data", "Avalonia.Markup"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.MarkupExtensions", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Styling", "Avalonia.Markup.Xaml"),
				CreateNamespaceInfo("Avalonia.Markup.Xaml.Templates", "Avalonia.Markup.Xaml")
			});
			dictionary.Add("x", new AvaloniaXamlIlXmlNamespaceInfo[0]);
			dictionary.Add("dialogs", new AvaloniaXamlIlXmlNamespaceInfo[1] { CreateNamespaceInfo("Avalonia.Dialogs", (string)null) });
			return dictionary;
		}

		static NamespaceInfo_003A_002FAboutAvaloniaDialog_002Examl()
		{
			Singleton = new NamespaceInfo_003A_002FAboutAvaloniaDialog_002Examl();
		}
	}
}
