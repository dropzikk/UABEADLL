using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Avalonia.Data.Core.Parsers;

[RequiresUnreferencedCode("ExpressionNode might require unreferenced code.")]
internal class ExpressionVisitorNodeBuilder : ExpressionVisitor
{
	private const string MultiDimensionalArrayGetterMethodName = "Get";

	private static PropertyInfo AvaloniaObjectIndexer;

	private static MethodInfo CreateDelegateMethod;

	private readonly bool _enableDataValidation;

	public List<ExpressionNode> Nodes { get; }

	static ExpressionVisitorNodeBuilder()
	{
		AvaloniaObjectIndexer = typeof(AvaloniaObject).GetProperty("Item", new Type[1] { typeof(AvaloniaProperty) });
		CreateDelegateMethod = typeof(MethodInfo).GetMethod("CreateDelegate", new Type[2]
		{
			typeof(Type),
			typeof(object)
		});
	}

	public ExpressionVisitorNodeBuilder(bool enableDataValidation)
	{
		_enableDataValidation = enableDataValidation;
		Nodes = new List<ExpressionNode>();
	}

	protected override Expression VisitUnary(UnaryExpression node)
	{
		if (node.NodeType == ExpressionType.Not && node.Type == typeof(bool))
		{
			Nodes.Add(new LogicalNotNode());
		}
		else if (node.NodeType == ExpressionType.Convert)
		{
			if (!node.Operand.Type.IsAssignableFrom(node.Type))
			{
				throw new ExpressionParseException(0, "Cannot parse non-inheritance casts in a binding expression.");
			}
		}
		else if (node.NodeType != ExpressionType.TypeAs)
		{
			throw new ExpressionParseException(0, $"Unable to parse unary operator {node.NodeType} in a binding expression");
		}
		return base.VisitUnary(node);
	}

	protected override Expression VisitMember(MemberExpression node)
	{
		Expression result = base.VisitMember(node);
		Nodes.Add(new PropertyAccessorNode(node.Member.Name, _enableDataValidation));
		return result;
	}

	protected override Expression VisitIndex(IndexExpression node)
	{
		Visit(node.Object);
		if (node.Indexer == AvaloniaObjectIndexer)
		{
			AvaloniaProperty argumentExpressionValue = GetArgumentExpressionValue<AvaloniaProperty>(node.Arguments[0]);
			Nodes.Add(new AvaloniaPropertyAccessorNode(argumentExpressionValue, _enableDataValidation));
		}
		else
		{
			Nodes.Add(new IndexerExpressionNode(node));
		}
		return node;
	}

	private static T GetArgumentExpressionValue<T>(Expression expr)
	{
		try
		{
			return Expression.Lambda<Func<T>>(expr, Array.Empty<ParameterExpression>()).Compile(preferInterpretation: true)();
		}
		catch (InvalidOperationException innerException)
		{
			throw new ExpressionParseException(0, "Unable to parse indexer value.", innerException);
		}
	}

	protected override Expression VisitBinary(BinaryExpression node)
	{
		if (node.NodeType == ExpressionType.ArrayIndex)
		{
			return Visit(Expression.MakeIndex(node.Left, null, new Expression[1] { node.Right }));
		}
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitBlock(BlockExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override CatchBlock VisitCatchBlock(CatchBlock node)
	{
		throw new ExpressionParseException(0, "Catch blocks are not allowed in binding expressions.");
	}

	protected override Expression VisitConditional(ConditionalExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitDynamic(DynamicExpression node)
	{
		throw new ExpressionParseException(0, "Dynamic expressions are not allowed in binding expressions.");
	}

	protected override ElementInit VisitElementInit(ElementInit node)
	{
		throw new ExpressionParseException(0, "Element init expressions are not valid in a binding expression.");
	}

	protected override Expression VisitGoto(GotoExpression node)
	{
		throw new ExpressionParseException(0, "Goto expressions not supported in binding expressions.");
	}

	protected override Expression VisitInvocation(InvocationExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitLabel(LabelExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitListInit(ListInitExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitLoop(LoopExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
	{
		throw new ExpressionParseException(0, "Member assignments not supported in binding expressions.");
	}

	protected override Expression VisitMethodCall(MethodCallExpression node)
	{
		if (node.Method == CreateDelegateMethod)
		{
			Visit(node.Arguments[1]);
			Nodes.Add(new PropertyAccessorNode(GetArgumentExpressionValue<MethodInfo>(node.Object).Name, _enableDataValidation));
			return node;
		}
		if (node.Method.Name == StreamBindingExtensions.StreamBindingName || node.Method.Name.StartsWith(StreamBindingExtensions.StreamBindingName + "`"))
		{
			if (node.Method.IsStatic)
			{
				Visit(node.Arguments[0]);
			}
			else
			{
				Visit(node.Object);
			}
			Nodes.Add(new StreamNode());
			return node;
		}
		PropertyInfo propertyInfo = TryGetPropertyFromMethod(node.Method);
		if (propertyInfo != null)
		{
			return Visit(Expression.MakeIndex(node.Object, propertyInfo, node.Arguments));
		}
		if (node.Object.Type.IsArray && node.Method.Name == "Get")
		{
			return Visit(Expression.MakeIndex(node.Object, null, node.Arguments));
		}
		throw new ExpressionParseException(0, $"Invalid method call in binding expression: '{node.Method.DeclaringType.AssemblyQualifiedName}.{node.Method.Name}'.");
	}

	private static PropertyInfo? TryGetPropertyFromMethod(MethodInfo method)
	{
		return method.DeclaringType?.GetRuntimeProperties().FirstOrDefault((PropertyInfo prop) => prop.GetMethod == method);
	}

	protected override Expression VisitSwitch(SwitchExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitTry(TryExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}

	protected override Expression VisitTypeBinary(TypeBinaryExpression node)
	{
		throw new ExpressionParseException(0, $"Invalid expression type in binding expression: {node.NodeType}.");
	}
}
