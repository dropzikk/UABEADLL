namespace Tmds.DBus.Protocol;

public sealed class MatchRule
{
	private MatchRuleData _data;

	internal MatchRuleData Data => _data;

	public MessageType? Type
	{
		get
		{
			return _data.MessageType;
		}
		set
		{
			_data.MessageType = value;
		}
	}

	public string? Sender
	{
		get
		{
			return _data.Sender;
		}
		set
		{
			_data.Sender = value;
		}
	}

	public string? Interface
	{
		get
		{
			return _data.Interface;
		}
		set
		{
			_data.Interface = value;
		}
	}

	public string? Member
	{
		get
		{
			return _data.Member;
		}
		set
		{
			_data.Member = value;
		}
	}

	public string? Path
	{
		get
		{
			return _data.Path;
		}
		set
		{
			_data.Path = value;
		}
	}

	public string? PathNamespace
	{
		get
		{
			return _data.PathNamespace;
		}
		set
		{
			_data.PathNamespace = value;
		}
	}

	public string? Destination
	{
		get
		{
			return _data.Destination;
		}
		set
		{
			_data.Destination = value;
		}
	}

	public string? Arg0
	{
		get
		{
			return _data.Arg0;
		}
		set
		{
			_data.Arg0 = value;
		}
	}

	public string? Arg0Path
	{
		get
		{
			return _data.Arg0Path;
		}
		set
		{
			_data.Arg0Path = value;
		}
	}

	public string? Arg0Namespace
	{
		get
		{
			return _data.Arg0Namespace;
		}
		set
		{
			_data.Arg0Namespace = value;
		}
	}

	public override string ToString()
	{
		return _data.GetRuleString();
	}
}
