using System;
using System.Collections.Generic;

namespace Avalonia.Controls.Utils;

public interface ICellEditBinding
{
	bool IsValid { get; }

	IEnumerable<Exception> ValidationErrors { get; }

	IObservable<bool> ValidationChanged { get; }

	bool CommitEdit();
}
