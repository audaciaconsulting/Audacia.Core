using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Audacia.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo(Expression propertyExpression)
        {
            if (propertyExpression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Selector must be lambda expression", nameof(propertyExpression));
            }

            var lambda = (LambdaExpression) propertyExpression;

            var memberExpression = ExtractMemberExpression(lambda.Body);

            if (memberExpression == null)
            {
                throw new ArgumentException("Selector must be member access expression", nameof(propertyExpression));
            }

            if (memberExpression.Member.DeclaringType == null)
            {
                throw new InvalidOperationException("Property does not have declaring type");
            }

            return memberExpression.Member.DeclaringType.GetProperty(memberExpression.Member.Name);
        }

        public static PropertyInfo GetPropertyInfo<T>(this T obj, Expression<Func<T, object>> propertyExpression)
        {
            //Overloaded so allow object specific property access and allow implicit type argument
            return GetPropertyInfo(propertyExpression);
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression) expression);
                case ExpressionType.Convert:
                    var operand = ((UnaryExpression) expression).Operand;
                    return ExtractMemberExpression(operand);
                default:
                    return null;
            }
        }

        public static Expression<Func<TTarget, TResult>> ConvertGenericTypeArgument<TSource, TTarget, TResult>(
            this Expression<Func<TSource, TResult>> root)
        {
            var visitor = new ParameterReplacer(typeof(TSource), typeof(TTarget));
            return visitor.Visit(root) as Expression<Func<TTarget, TResult>>;
        }

        public static LambdaExpression ConvertGenericTypeArgument<TSource, TResult>(
            this Expression<Func<TSource, TResult>> root, Type targetType)
        {
            var visitor = new ParameterReplacer(typeof(TSource), targetType);
            var expression = visitor.Visit(root);
            return expression as LambdaExpression;
        }


        public static TResult Execute<T, TResult>(this Expression<Func<T, TResult>> expression, T arg)
        {
            return expression.Compile()(arg);
        }
        
        public static void Perform<T>(this Expression<Action<T>> expression, T arg)
        {
            expression.Compile()(arg);
        }
        
        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }
        
        /// <summary>
        /// Apply the <see cref="second"/> expression to the result of this expression.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TInter"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="first"></param>
        /// <param name="second">The second expression to apply to the result of the first</param>
        /// <returns></returns>
        public static Expression<Func<TIn, TOut>> Then<TIn, TInter, TOut>(this Expression<Func<TIn, TInter>> first, Expression<Func<TInter, TOut>> second)
        {
            //Map the parameters of the second expression to the body of the first
            var replacements = second.Parameters
                .Select(p => new { parameter = p, replacement = first.Body })
                .ToDictionary(p => (Expression)p.parameter, p => p.replacement);
            
            //Replace the parameters of the second Expression with the body of the first
            var secondBody = ParameterRebinder.ReplaceParameters(replacements, second.Body);
            return Expression.Lambda<Func<TIn, TOut>>(secondBody, first.Parameters);
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new { f = (Expression)f, s = (Expression)second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly IDictionary<Expression, Expression> _map;

            private ParameterRebinder(Dictionary<Expression, Expression> map)
            {
                _map = map ?? new Dictionary<Expression, Expression>();
            }

            public static Expression ReplaceParameters(Dictionary<Expression, Expression> map,
                Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                return _map.TryGetValue(p, out var replacement)
                    ? replacement
                    : base.VisitParameter(p);
            }
        }
        
        private class ParameterReplacer : ExpressionVisitor
        {
            private ReadOnlyCollection<ParameterExpression> _parameters;
            private readonly Type _source;
            private readonly Type _target;

            public ParameterReplacer(Type source, Type target)
            {
                _source = source;
                _target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameters?.FirstOrDefault(p => p.Name == node.Name) ??
                       (node.Type == _source ? Expression.Parameter(_target, node.Name) : node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                _parameters = VisitAndConvert(node.Parameters, nameof(VisitLambda));
                return Expression.Lambda(Visit(node.Body), _parameters);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                return node.Member.DeclaringType == _source
                    ? Expression.Property(Visit(node.Expression), node.Member.Name)
                    : base.VisitMember(node);
            }
        }
    }
}