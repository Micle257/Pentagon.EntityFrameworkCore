// -----------------------------------------------------------------------
//  <copyright file="NumberFilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;

    public static class NumberFilterExpressionHelper<T>
            where T : IComparable
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity>(Expression<Func<TEntity, T>> propertySelector, NumberFilter filter, T value)
        {
            var ex = GetFilterCallback(propertySelector.Body, filter, value);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            ex = new ParameterReplacer(parameter).Visit(ex);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), ex, parameter);
        }

        public static Expression<Func<TEntity, bool>> GetDoubleFilter<TEntity>(Expression<Func<TEntity, T>> propertySelector,
                                                                               NumberFilter firstFilter,
                                                                               T firstValue,
                                                                               FilterLogicOperation operation,
                                                                               NumberFilter secondFilter,
                                                                               T secondValue)
        {
            var leftExpression = GetFilter(propertySelector, firstFilter, firstValue).Body;

            var rightExpression = GetFilter(propertySelector, secondFilter, secondValue).Body;

            ExpressionType expressionType;

            switch (operation)
            {
                case FilterLogicOperation.Or:
                    expressionType = ExpressionType.OrElse;
                    break;
                case FilterLogicOperation.And:
                    expressionType = ExpressionType.AndAlso;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            Expression expression = Expression.MakeBinary(expressionType, leftExpression, rightExpression);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            expression = new ParameterReplacer(parameter).Visit(expression);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), expression, parameter);
        }

        static Expression GetFilterCallback(Expression callBody, NumberFilter textFilter, T value)
        {
            switch (textFilter)
            {
                case NumberFilter.Equal:
                    return GetNotInvertedBody(callBody, v => v.Equals(value));
                case NumberFilter.NotEqual:
                    return GetInvertedBody(callBody, v => !v.Equals(value));
                case NumberFilter.GreatenThan:
                    return GetBinaryBody(callBody, v => v.CompareTo(value) < 0);
                case NumberFilter.GreatenThenOrEqualTo:
                    return GetDoubleBinaryBody(callBody, v => v.CompareTo(value) < 0 || v.Equals(value));
                case NumberFilter.LessThen:
                    return GetBinaryBody(callBody, v => v.CompareTo(value) > 0);
                case NumberFilter.LessThenOrEqualTo:
                    return GetDoubleBinaryBody(callBody, v => v.CompareTo(value) > 0 || v.Equals(value));
                default:
                    throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
            }
        }

        static Expression GetDoubleBinaryBody(Expression callBody, Expression<Func<T, bool>> callback)
        {
            var leftExpression = GetBinaryBody(callBody, Expression.Lambda<Func<T, bool>>(((BinaryExpression) callback.Body).Left));

            var rightExpression = GetNotInvertedBody(callBody, Expression.Lambda<Func<T, bool>>(((BinaryExpression) callback.Body).Right));

            return Expression.MakeBinary(ExpressionType.OrElse, leftExpression, rightExpression);
        }

        static Expression GetBinaryBody(Expression callBody, Expression<Func<T, bool>> callback)
        {
            var binaryBody = (BinaryExpression) callback.Body;

            var body = (MethodCallExpression) binaryBody.Left;
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            Expression concatExpressionBody = Expression.Call(callBody, containsMethodInfo, containsArgument);

            concatExpressionBody = Expression.MakeBinary(binaryBody.NodeType, concatExpressionBody, binaryBody.Right);

            return concatExpressionBody;
        }

        static Expression GetNotInvertedBody(Expression callBody, Expression<Func<T, bool>> callback)
        {
            var body = (MethodCallExpression) callback.Body;
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            var concatExpressionBody = Expression.Call(callBody, containsMethodInfo, containsArgument);

            return concatExpressionBody;
        }

        static Expression GetInvertedBody(Expression callBody, Expression<Func<T, bool>> callback)
        {
            var body = (MethodCallExpression) ((UnaryExpression) callback.Body).Operand;
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            var concatExpressionBody = Expression.Not(Expression.Call(callBody, containsMethodInfo, containsArgument));

            return concatExpressionBody;
        }
    }
}