using System;
using System.Collections.Generic;
using System.Text;
using TextMateSharp.Internal.Grammars.Parser;
using TextMateSharp.Internal.Themes;

namespace TextMateSharp.Internal.Parser;

public class PList<T>
{
	private bool theme;

	private List<string> errors;

	private PListObject currObject;

	private T result;

	private StringBuilder text;

	public PList(bool theme)
	{
		this.theme = theme;
		errors = new List<string>();
		currObject = null;
	}

	public void StartElement(string tagName)
	{
		if ("dict".Equals(tagName))
		{
			currObject = Create(currObject, valueAsArray: false);
		}
		else if ("array".Equals(tagName))
		{
			currObject = Create(currObject, valueAsArray: true);
		}
		else if ("key".Equals(tagName) && currObject != null)
		{
			currObject.SetLastKey(null);
		}
		if (text == null)
		{
			text = new StringBuilder("");
		}
		text.Clear();
	}

	private PListObject Create(PListObject parent, bool valueAsArray)
	{
		if (theme)
		{
			return new PListTheme(parent, valueAsArray);
		}
		return new PListGrammar(parent, valueAsArray);
	}

	public void EndElement(string tagName)
	{
		object value = null;
		string text = this.text.ToString();
		if ("key".Equals(tagName))
		{
			if (currObject == null || currObject.IsValueAsArray())
			{
				errors.Add("key can only be used inside an open dict element");
			}
			else
			{
				currObject.SetLastKey(text);
			}
			return;
		}
		if ("dict".Equals(tagName) || "array".Equals(tagName))
		{
			if (currObject == null)
			{
				errors.Add(tagName + " closing tag found, without opening tag");
				return;
			}
			value = currObject.GetValue();
			currObject = currObject.parent;
		}
		else if ("string".Equals(tagName) || "data".Equals(tagName))
		{
			value = text;
		}
		else if (!"date".Equals(tagName))
		{
			if ("integer".Equals(tagName))
			{
				try
				{
					value = int.Parse(text);
				}
				catch (Exception)
				{
					errors.Add(text + " is not a integer");
					return;
				}
			}
			else if ("real".Equals(tagName))
			{
				try
				{
					value = float.Parse(text);
				}
				catch (Exception)
				{
					errors.Add(text + " is not a float");
					return;
				}
			}
			else if ("true".Equals(tagName))
			{
				value = true;
			}
			else
			{
				if (!"false".Equals(tagName))
				{
					if (!"plist".Equals(tagName))
					{
						errors.Add("Invalid tag name: " + tagName);
					}
					return;
				}
				value = false;
			}
		}
		if (currObject == null)
		{
			result = (T)value;
		}
		else if (currObject.IsValueAsArray())
		{
			currObject.AddValue(value);
		}
		else if (currObject.GetLastKey() != null)
		{
			currObject.AddValue(value);
		}
		else
		{
			errors.Add("Dictionary key missing for value " + value);
		}
	}

	public void AddString(string str)
	{
		text.Append(str);
	}

	public T GetResult()
	{
		return result;
	}
}
