using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Avalonia.Data.Core.Parsers;
using Avalonia.Data.Core.Plugins;
using Avalonia.Reactive;

namespace Avalonia.Data.Core;

internal class ExpressionObserver : LightweightObservableBase<object?>, IDescription
{
	public static readonly List<IPropertyAccessorPlugin> PropertyAccessors = new List<IPropertyAccessorPlugin>
	{
		new AvaloniaPropertyAccessorPlugin(),
		new MethodAccessorPlugin(),
		new InpcPropertyAccessorPlugin()
	};

	public static readonly List<IDataValidationPlugin> DataValidators = new List<IDataValidationPlugin>
	{
		new DataAnnotationsValidationPlugin(),
		new IndeiValidationPlugin(),
		new ExceptionValidationPlugin()
	};

	public static readonly List<IStreamPlugin> StreamHandlers = new List<IStreamPlugin>
	{
		new TaskStreamPlugin(),
		new ObservableStreamPlugin()
	};

	private readonly ExpressionNode _node;

	private object? _root;

	private Func<object?>? _rootGetter;

	private IDisposable? _rootSubscription;

	private WeakReference<object?>? _value;

	private IReadOnlyList<ITransformNode>? _transformNodes;

	private IReadOnlyList<ITransformNode> TransformNodes => _transformNodes ?? (_transformNodes = GetTransformNodesFromChain());

	public string? Description { get; }

	public string? Expression { get; }

	public Type? ResultType => (Leaf as SettableNode)?.PropertyType;

	private ExpressionNode Leaf
	{
		get
		{
			ExpressionNode expressionNode = _node;
			while (expressionNode.Next != null)
			{
				expressionNode = expressionNode.Next;
			}
			return expressionNode;
		}
	}

	public ExpressionObserver(object? root, ExpressionNode node, string? description = null)
	{
		_node = node;
		Description = description;
		_root = new WeakReference<object>((root == AvaloniaProperty.UnsetValue) ? null : root);
	}

	public ExpressionObserver(IObservable<object?> rootObservable, ExpressionNode node, string? description)
	{
		if (rootObservable == null)
		{
			throw new ArgumentNullException("rootObservable");
		}
		_node = node;
		Description = description;
		_root = rootObservable;
	}

	public ExpressionObserver(Func<object?> rootGetter, ExpressionNode node, IObservable<ValueTuple> update, string? description)
	{
		Description = description;
		_rootGetter = rootGetter ?? throw new ArgumentNullException("rootGetter");
		_node = node ?? throw new ArgumentNullException("node");
		_root = update.Select((ValueTuple x) => rootGetter());
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Typed Expressions preserves members used in the expression tree.")]
	public static ExpressionObserver Create<T, U>(T? root, Expression<Func<T, U>> expression, bool enableDataValidation = false, string? description = null)
	{
		return new ExpressionObserver(root, Parse(expression, enableDataValidation), description ?? expression.ToString());
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Typed Expressions preserves members used in the expression tree.")]
	public static ExpressionObserver Create<T, U>(IObservable<T> rootObservable, Expression<Func<T, U>> expression, bool enableDataValidation = false, string? description = null)
	{
		if (rootObservable == null)
		{
			throw new ArgumentNullException("rootObservable");
		}
		return new ExpressionObserver(rootObservable.Select((Func<T, object>)((T o) => o)), Parse(expression, enableDataValidation), description ?? expression.ToString());
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Typed Expressions preserves members used in the expression tree.")]
	public static ExpressionObserver Create<T, U>(Func<T> rootGetter, Expression<Func<T, U>> expression, IObservable<ValueTuple> update, bool enableDataValidation = false, string? description = null)
	{
		if (rootGetter == null)
		{
			throw new ArgumentNullException("rootGetter");
		}
		return new ExpressionObserver(() => rootGetter(), Parse(expression, enableDataValidation), update, description ?? expression.ToString());
	}

	private IReadOnlyList<ITransformNode> GetTransformNodesFromChain()
	{
		LinkedList<ITransformNode> linkedList = new LinkedList<ITransformNode>();
		for (ExpressionNode expressionNode = _node; expressionNode != null; expressionNode = expressionNode.Next)
		{
			if (expressionNode is ITransformNode value)
			{
				linkedList.AddFirst(value);
			}
		}
		return new List<ITransformNode>(linkedList);
	}

	public bool SetValue(object? value, BindingPriority priority = BindingPriority.LocalValue)
	{
		if (Leaf is SettableNode settableNode)
		{
			foreach (ITransformNode transformNode in TransformNodes)
			{
				value = transformNode.Transform(value);
				if (value is BindingNotification)
				{
					return false;
				}
			}
			return settableNode.SetTargetValue(value, priority);
		}
		return false;
	}

	protected override void Initialize()
	{
		_value = null;
		if (_rootGetter != null)
		{
			_node.Target = new WeakReference<object>(_rootGetter());
		}
		_node.Subscribe(ValueChanged);
		StartRoot();
	}

	protected override void Deinitialize()
	{
		_rootSubscription?.Dispose();
		_rootSubscription = null;
		_node.Unsubscribe();
	}

	protected override void Subscribed(IObserver<object?> observer, bool first)
	{
		if (!first && _value != null && _value.TryGetTarget(out object target))
		{
			observer.OnNext(target);
		}
	}

	[RequiresUnreferencedCode("ExpressionNode might require unreferenced code.")]
	private static ExpressionNode Parse(LambdaExpression expression, bool enableDataValidation)
	{
		return ExpressionTreeParser.Parse(expression, enableDataValidation);
	}

	private void StartRoot()
	{
		if (_root is IObservable<object> observable)
		{
			_rootSubscription = observable.Subscribe(new AnonymousObserver<object>(delegate(object x)
			{
				_node.Target = new WeakReference<object>((x != AvaloniaProperty.UnsetValue) ? x : null);
			}, delegate
			{
				PublishCompleted();
			}, base.PublishCompleted));
		}
		else
		{
			_node.Target = (WeakReference<object>)_root;
		}
	}

	private void ValueChanged(object? value)
	{
		(BindingNotification.ExtractError(value) as MarkupBindingChainException)?.Commit(Description ?? "{empty}");
		_value = new WeakReference<object>(value);
		PublishNext(value);
	}
}
