using System;
using System.Text;

namespace Tmds.DBus.Protocol;

internal struct MatchRuleData
{
	public MessageType? MessageType { get; set; }

	public string? Sender { get; set; }

	public string? Interface { get; set; }

	public string? Member { get; set; }

	public string? Path { get; set; }

	public string? PathNamespace { get; set; }

	public string? Destination { get; set; }

	public string? Arg0 { get; set; }

	public string? Arg0Path { get; set; }

	public string? Arg0Namespace { get; set; }

	public string GetRuleString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (MessageType.HasValue)
		{
			string text = MessageType switch
			{
				Tmds.DBus.Protocol.MessageType.MethodCall => "type=method_call", 
				Tmds.DBus.Protocol.MessageType.MethodReturn => "type=method_return", 
				Tmds.DBus.Protocol.MessageType.Error => "type=error", 
				Tmds.DBus.Protocol.MessageType.Signal => "type=signal", 
				_ => null, 
			};
			if (text != null)
			{
				stringBuilder.Append(text);
			}
		}
		Append(stringBuilder, "sender", Sender);
		Append(stringBuilder, "interface", Interface);
		Append(stringBuilder, "member", Member);
		Append(stringBuilder, "path", Path);
		Append(stringBuilder, "pathNamespace", PathNamespace);
		Append(stringBuilder, "destination", Destination);
		Append(stringBuilder, "arg0", Arg0);
		Append(stringBuilder, "arg0Path", Arg0Path);
		Append(stringBuilder, "arg0Namespace", Arg0Namespace);
		return stringBuilder.ToString();
		static void Append(StringBuilder sb, string key, string? value)
		{
			if (value != null)
			{
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, sb);
				handler.AppendFormatted((sb.Length > 0) ? ((object)',') : "");
				handler.AppendFormatted(key);
				handler.AppendLiteral("=");
				sb.Append(ref handler);
				bool flag = false;
				ReadOnlySpan<char> readOnlySpan = value.AsSpan();
				while (!readOnlySpan.IsEmpty)
				{
					int num = readOnlySpan.IndexOfAny(new char[2] { ',', '\'' });
					if (num == -1)
					{
						sb.Append(readOnlySpan);
						break;
					}
					bool flag2 = readOnlySpan[num] == ',';
					if ((flag2 && !flag) || (!flag2 && flag))
					{
						sb.Append("'");
						flag = !flag;
					}
					sb.Append(readOnlySpan.Slice(0, num + (flag2 ? 1 : 0)));
					if (!flag2)
					{
						sb.Append("\\'");
					}
					readOnlySpan = readOnlySpan.Slice(num + 1);
				}
				if (flag)
				{
					sb.Append("'");
					flag = false;
				}
			}
		}
	}
}
