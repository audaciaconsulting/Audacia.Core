using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Audacia.Core.Extensions.Helpers;

namespace Audacia.Core.Extensions
{
    /// <summary>
    /// Extension methods for the type <see cref="LambdaExpression"/>.
    /// </summary>
    public static class LambdaExpressionExtensions
    {
        /// <summary>
        /// Convert generic type argument in an expression to a specified type.
        /// </summary>
        /// <typeparam name="TSource">The generic type.</typeparam>
        /// <param name="root">The original expression.</param>
        /// <param name="targetType">The type to replace <typeparamref name="TSource" />.</param>
        /// <returns>An expression with <typeparamref name="TSource" /> replaced with <paramref name="targetType" />.</returns>
        public static LambdaExpression ConvertGenericTypeArgument<TSource>(
            this LambdaExpression root,
            Type targetType)
        {
            var visitor = new ParameterReplacer(typeof(TSource), targetType);
            var expression = visitor.Visit(root);
            return expression as LambdaExpression ?? default!;
        }

        /// <summary>
        /// Get the property info from the provided lambda expression.
        /// </summary>
        /// <param name="expression">The lambda expression.</param>
        /// <returns>The property info of the getter.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">If the expression isn't a getter to a property on the source of the lambda.</exception>
        internal static PropertyInfo GetPropertyInfo(this LambdaExpression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return expression.Body switch
            {
                null => throw new ArgumentNullException(nameof(expression)),
                UnaryExpression { Operand: MemberExpression me } => (PropertyInfo)me.Member,
                MemberExpression me => (PropertyInfo)me.Member,
                _ => throw new ArgumentException($"The expression doesn't indicate a valid property. [ {expression} ]")
            };
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        /// <param name="first">The source expression.</param>
        /// <param name="second">The expression to join on to <paramref name="first" />.</param>
        /// <returns>An expression combining <paramref name="first" /> and <paramref name="second" />.</returns>
        public static LambdaExpression And(
            this LambdaExpression first,
            LambdaExpression second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        /// <param name="first">The source expression.</param>
        /// <param name="second">The expression to join on to <paramref name="first"/>.</param>
        /// <returns>An expression combining <paramref name="first"/> or <paramref name="second"/>.</returns>
        public static LambdaExpression Or(
            this LambdaExpression first,
            LambdaExpression second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>Merges two expressions into one.</summary>
        /// <param name="first">The source expression.</param>
        /// <param name="second">The expression to merge with <paramref name="first" />.</param>
        /// <param name="merge">How the expressions are merged together.</param>
        /// <returns>The merged expression of <paramref name="first" /> and <paramref name="second" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="first" /> or <paramref name="second" /> is null.</exception>
        public static LambdaExpression Compose(
            this LambdaExpression first,
            LambdaExpression second,
            Func<Expression, Expression, Expression> merge)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (merge == null)
                throw new ArgumentNullException(nameof(merge));
            var map = first.Parameters.Select((f, i) => new
            {
                first = (Expression)f,
                second = (Expression)second.Parameters[i]
            }).ToDictionary(p => p.second, p => p.first);
            Expression expression = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda(merge(first.Body, expression), first.Parameters);
        }
    }
}