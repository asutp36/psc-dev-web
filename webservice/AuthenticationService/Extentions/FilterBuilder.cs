using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthenticationService.Extentions
{
    //public static class FilterBuilder
    //{
    //    public static string[] TokenizeQuery(string query)
    //    {
    //        return string.IsNullOrEmpty(query)
    //            ? Array.Empty<string>()
    //            : query.Trim().Split(new[] { " ", ";" }, StringSplitOptions.RemoveEmptyEntries);
    //    }

    //    public static (string Prefix, string Value) PrefixedToken(string token)
    //    {
    //        var parts = token.Split(':', 2);
    //        return parts.Length < 2 ? ("", parts[0]) : (parts[0].ToLowerInvariant(), parts[1]);
    //    }
    //}

    //public class FilterBuilder<T>
    //{
    //    public Expression<Func<T, bool>> Expression { get; private set; }

    //    public FilterBuilder()
    //    {
    //        Expression = _ => true;
    //    }

    //    public FilterBuilder(Expression<Func<T, bool>> expr)
    //    {
    //        Expression = expr;
    //    }

    //    public FilterBuilder<T> AndQuery(
    //        string query,
    //        Func<string, Expression<Func<T, bool>>> queryExprBuilder)
    //    {
    //        return AndQuery(query, 1, queryExprBuilder);
    //    }

    //    public FilterBuilder<T> AndQuery(
    //        string query,
    //        int queryMinLength,
    //        Func<string, Expression<Func<T, bool>>> queryExprBuilder)
    //    {
    //        var tokens = FilterBuilder.TokenizeQuery(query);
    //        if (tokens.Sum(t => t.Length) >= queryMinLength)
    //        {
    //            Expression = Expression.AndAlso(BuildQueryExpression(tokens, queryExprBuilder));
    //        }

    //        return this;
    //    }

    //    public FilterBuilder<T> And(Expression<Func<T, bool>> expr)
    //    {
    //        Expression = Expression.AndAlso(expr);
    //        return this;
    //    }

    //    public ConditionFilterBuilder<T> If(bool condition)
    //    {
    //        return new ConditionFilterBuilder<T>(condition, this);
    //    }

    //    public NullableValueFilterBuilder<T, TValue> HasValue<TValue>(TValue? nullableValue)
    //        where TValue : struct
    //    {
    //        return new NullableValueFilterBuilder<T, TValue>(nullableValue, this);
    //    }

    //    public static FilterBuilder<T> FromQuery(
    //        string query,
    //        Func<string, Expression<Func<T, bool>>> queryExprBuilder)
    //    {
    //        return FromQuery(query, 1, queryExprBuilder);
    //    }

    //    public static FilterBuilder<T> FromQuery(
    //        string query,
    //        int queryMinLength,
    //        Func<string, Expression<Func<T, bool>>> queryExprBuilder)
    //    {
    //        var tokens = FilterBuilder.TokenizeQuery(query);
    //        if (!tokens.Any()) return new FilterBuilder<T>();
    //        return tokens.Sum(t => t.Length) >= queryMinLength
    //            ? new FilterBuilder<T>(BuildQueryExpression(tokens, queryExprBuilder))
    //            : new FilterBuilder<T>(_ => false);
    //    }

    //    private static Expression<Func<T, bool>> BuildQueryExpression(string[] tokens, Func<string, Expression<Func<T, bool>>> queryExprBuilder)
    //    {
    //        var queryExpr = queryExprBuilder(tokens.First());
    //        if (tokens.Length < 2) return queryExpr;

    //        foreach (var token in tokens.Skip(1))
    //        {
    //            queryExpr = queryExpr.AndAlso(queryExprBuilder(token));
    //        }

    //        return queryExpr;
    //    }

    //    public static implicit operator Expression<Func<T, bool>>(FilterBuilder<T> builder) => builder.Expression;
    //}

    //public class ConditionFilterBuilder<T>
    //{
    //    protected readonly bool Condition;
    //    protected readonly FilterBuilder<T> Builder;

    //    internal ConditionFilterBuilder(bool condition, FilterBuilder<T> builder)
    //    {
    //        Condition = condition;
    //        Builder = builder;
    //    }

    //    public FilterBuilder<T> ThenAnd(Expression<Func<T, bool>> expr)
    //    {
    //        return Condition ? Builder.And(expr) : Builder;
    //    }

    //    public FilterBuilder<T> ThenAndQuery(
    //        string query,
    //        Func<string, Expression<Func<T, bool>>> queryExprBuilder)
    //    {
    //        return Condition ? Builder.AndQuery(query, queryExprBuilder) : Builder;
    //    }
    //}

    //public class NullableValueFilterBuilder<T, TValue> : ConditionFilterBuilder<T> where TValue : struct
    //{
    //    private readonly TValue? _nullableValue;

    //    internal NullableValueFilterBuilder(TValue? nullableValue, FilterBuilder<T> builder) :
    //        base(nullableValue.HasValue, builder)
    //    {
    //        _nullableValue = nullableValue;
    //    }

    //    public FilterBuilder<T> ThenAndEqual(Expression<Func<T, TValue?>> keySelector) =>
    //   Check(keySelector, ExpressionExtension.Equal);

    //    public FilterBuilder<T> ThenAndLess(Expression<Func<T, TValue?>> keySelector) =>
    //        Check(keySelector, ExpressionExtension.LessThan);

    //    public FilterBuilder<T> ThenAndLessOrEqual(Expression<Func<T, TValue?>> keySelector) =>
    //        Check(keySelector, ExpressionExtension.LessThanOrEqual);

    //    public FilterBuilder<T> ThenAndGreater(Expression<Func<T, TValue?>> keySelector) =>
    //        Check(keySelector, ExpressionExtension.GreaterThan);

    //    public FilterBuilder<T> ThenAndGreaterOrEqual(Expression<Func<T, TValue?>> keySelector) =>
    //        Check(keySelector, ExpressionExtension.GreaterThanOrEqual);

    //    private FilterBuilder<T> Check(
    //        Expression<Func<T, TValue?>> keySelector,
    //        Func<Expression<Func<T, TValue?>>, Expression<Func<T, TValue?>>, Expression<Func<T, bool>>> exprBuilder) =>
    //        !Condition ? Builder : Builder.And(exprBuilder(_ => _nullableValue, keySelector));
    //}
}
