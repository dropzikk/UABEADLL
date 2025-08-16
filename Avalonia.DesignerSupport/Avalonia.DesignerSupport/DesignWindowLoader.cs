using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Embedding.Offscreen;
using Avalonia.Controls.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.DesignerSupport;

public class DesignWindowLoader
{
	public static Window LoadDesignerWindow(string xaml, string assemblyPath, string xamlFileProjectPath)
	{
		return LoadDesignerWindow(xaml, assemblyPath, xamlFileProjectPath, 1.0);
	}

	public static Window LoadDesignerWindow(string xaml, string assemblyPath, string xamlFileProjectPath, double renderScaling)
	{
		Window window;
		using (PlatformManager.DesignerMode())
		{
			AvaloniaXamlLoader.IRuntimeXamlLoader requiredService = AvaloniaLocator.Current.GetRequiredService<AvaloniaXamlLoader.IRuntimeXamlLoader>();
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
			Uri baseUri = null;
			if (assemblyPath != null)
			{
				if (xamlFileProjectPath == null)
				{
					xamlFileProjectPath = "/Designer/Fake.xaml";
				}
				baseUri = new Uri("avares://" + Path.GetFileNameWithoutExtension(assemblyPath) + xamlFileProjectPath);
			}
			Assembly assembly = ((assemblyPath != null) ? Assembly.LoadFile(Path.GetFullPath(assemblyPath)) : null);
			string value = assembly?.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault((AssemblyMetadataAttribute a) => a.Key == "AvaloniaUseCompiledBindingsByDefault")?.Value;
			bool result;
			object obj = requiredService.Load(new RuntimeXamlLoaderDocument(baseUri, stream), new RuntimeXamlLoaderConfiguration
			{
				LocalAssembly = assembly,
				DesignMode = true,
				UseCompiledBindingsByDefault = (bool.TryParse(value, out result) && result)
			});
			IStyle style = obj as IStyle;
			ResourceDictionary resourceDictionary = obj as ResourceDictionary;
			Control control;
			if (style != null)
			{
				Control previewWith = Design.GetPreviewWith((AvaloniaObject)style);
				if (previewWith != null)
				{
					previewWith.Styles.Add(style);
					control = previewWith;
				}
				else
				{
					control = new StackPanel
					{
						Children = 
						{
							(Control)new TextBlock
							{
								Text = "Styles can't be previewed without Design.PreviewWith. Add"
							},
							(Control)new TextBlock
							{
								Text = "<Design.PreviewWith>"
							},
							(Control)new TextBlock
							{
								Text = "    <Border Padding=20><!-- YOUR CONTROL FOR PREVIEW HERE --></Border>"
							},
							(Control)new TextBlock
							{
								Text = "</Design.PreviewWith>"
							},
							(Control)new TextBlock
							{
								Text = "before setters in your first Style"
							}
						}
					};
				}
			}
			else if (resourceDictionary == null)
			{
				control = ((!(obj is Application)) ? ((Control)obj) : new TextBlock
				{
					Text = "Application can't be previewed in design view"
				});
			}
			else
			{
				Control previewWith2 = Design.GetPreviewWith(resourceDictionary);
				if (previewWith2 != null)
				{
					previewWith2.Resources.MergedDictionaries.Add(resourceDictionary);
					control = previewWith2;
				}
				else
				{
					control = new StackPanel
					{
						Children = 
						{
							(Control)new TextBlock
							{
								Text = "ResourceDictionaries can't be previewed without Design.PreviewWith. Add"
							},
							(Control)new TextBlock
							{
								Text = "<Design.PreviewWith>"
							},
							(Control)new TextBlock
							{
								Text = "    <Border Padding=20><!-- YOUR CONTROL FOR PREVIEW HERE --></Border>"
							},
							(Control)new TextBlock
							{
								Text = "</Design.PreviewWith>"
							},
							(Control)new TextBlock
							{
								Text = "in your resource dictionary"
							}
						}
					};
				}
			}
			window = control as Window;
			if (window == null)
			{
				window = new Window
				{
					Content = control
				};
			}
			if (window.PlatformImpl is OffscreenTopLevelImplBase offscreenTopLevelImplBase)
			{
				offscreenTopLevelImplBase.RenderScaling = renderScaling;
			}
			Design.ApplyDesignModeProperties(window, control);
			if (!window.IsSet(Window.SizeToContentProperty))
			{
				if (double.IsNaN(window.Width))
				{
					window.SizeToContent |= SizeToContent.Width;
				}
				if (double.IsNaN(window.Height))
				{
					window.SizeToContent |= SizeToContent.Height;
				}
			}
		}
		window.Show();
		return window;
	}
}
