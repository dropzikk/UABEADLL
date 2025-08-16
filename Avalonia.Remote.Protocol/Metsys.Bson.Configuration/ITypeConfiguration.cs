using System;
using System.Linq.Expressions;

namespace Metsys.Bson.Configuration;

public interface ITypeConfiguration<T>
{
	ITypeConfiguration<T> UseAlias(Expression<Func<T, object>> expression, string alias);

	ITypeConfiguration<T> Ignore(Expression<Func<T, object>> expression);

	ITypeConfiguration<T> Ignore(string name);

	ITypeConfiguration<T> IgnoreIfNull(Expression<Func<T, object>> expression);
}
