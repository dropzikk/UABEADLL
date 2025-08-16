using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Controls.Selection;

public static class SelectionModelExtensions
{
	public record struct BatchUpdateOperation(ISelectionModel owner) : IDisposable
	{
		private readonly ISelectionModel _owner = owner;

		private bool _isDisposed = false;

		public BatchUpdateOperation(ISelectionModel owner)
		{
			owner.BeginBatchUpdate();
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				owner?.EndBatchUpdate();
				_isDisposed = true;
			}
		}

		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	public static IDisposable BatchUpdate(this ISelectionModel model)
	{
		return new BatchUpdateOperation(model);
	}
}
