using System;
using System.Linq.Expressions;

namespace Metsys.Bson.Configuration;

internal class TypeConfiguration<T> : ITypeConfiguration<T>
{
	private readonly BsonConfiguration _configuration;

	internal TypeConfiguration(BsonConfiguration configuration)
	{
		_configuration = configuration;
	}

	public ITypeConfiguration<T> UseAlias(Expression<Func<T, object>> expression, string alias)
	{
		MemberExpression memberExpression = expression.GetMemberExpression();
		_configuration.AddMap<T>(memberExpression.GetName(), alias);
		return this;
	}

	public ITypeConfiguration<T> Ignore(Expression<Func<T, object>> expression)
	{
		MemberExpression memberExpression = expression.GetMemberExpression();
		return Ignore(memberExpression.GetName());
	}

	public ITypeConfiguration<T> Ignore(string name)
	{
		_configuration.AddIgnore<T>(name);
		return this;
	}

	public ITypeConfiguration<T> IgnoreIfNull(Expression<Func<T, object>> expression)
	{
		MemberExpression memberExpression = expression.GetMemberExpression();
		_configuration.AddIgnoreIfNull<T>(memberExpression.GetName());
		return this;
	}
}
