using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Audacia.Core.Extensions.Helpers;

/// <summary>
/// Expression visitor that replaces an expression parameter with another of a different type, for use in manipulating an expression.
/// </summary>
/// <remarks>
/// <para>Constructs a new <see cref="ParameterReplacer"/>.</para>
/// </remarks>
/// <param name="source">The type of the expression parameter to replace.</param>
/// <param name="target">The destination type to replace the parameter with.</param>
internal class ParameterReplacer(Type source, Type target) : ExpressionVisitor
{
    private readonly Type _source = source;
    private readonly Type _target = target;
    private ReadOnlyCollection<ParameterExpression> _parameters = default!;

    /// <inheritdoc />
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return _parameters?.FirstOrDefault(p => p.Name == node.Name) ??
               (node.Type == _source ? Expression.Parameter(_target, node.Name) : node);
    }

    /// <inheritdoc />
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        _parameters = VisitAndConvert(node.Parameters, nameof(VisitLambda));
        var nodeParameters = _parameters;
        return Expression.Lambda(Visit(node.Body), nodeParameters);
    }

    /// <inheritdoc />
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        if (node.Member.DeclaringType == _source)
        {
            var expression = Visit(node?.Expression);

            if (expression != null)
            {
                return Expression.Property(expression, node!.Member.Name);
            }
        }

        return base.VisitMember(node!);
    }
}