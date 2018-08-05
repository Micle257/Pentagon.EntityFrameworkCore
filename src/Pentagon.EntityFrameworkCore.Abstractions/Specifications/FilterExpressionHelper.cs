// -----------------------------------------------------------------------
//  <copyright file="FilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;

    public static class FilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetTextFilter<TEntity>(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value)
        {
            var ex = GetTextFilterCallback(propertySelector.Body, filter, value);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            ex = new ParameterReplacer(parameter).Visit(ex);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), ex, parameter);
        }

        public static Expression<Func<TEntity, bool>> GetTextDoubleFilter<TEntity>(Expression<Func<TEntity, string>> propertySelector,
                                                                                   TextFilter firstFilter,
                                                                                   string firstValue,
                                                                                   FilterLogicOperation operation,
                                                                                   TextFilter secondFilter,
                                                                                   string secondValue)
        {
            var leftExpression = GetTextFilter(propertySelector, firstFilter, firstValue).Body;

            var rightExpression = GetTextFilter(propertySelector, secondFilter, secondValue).Body;

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

        static Expression GetTextFilterCallback(Expression callBody, TextFilter textFilter, string value)
        {
            switch (textFilter)
            {
                case TextFilter.Equal:
                    return GetNotInvertedBody(callBody, v => v.Equals(value));
                case TextFilter.NotEqual:
                    return GetInvertedBody(callBody, v => !v.Equals(value));
                case TextFilter.StartWith:
                    return GetNotInvertedBody(callBody, v => v.StartsWith(value));
                case TextFilter.EndWith:
                    return GetNotInvertedBody(callBody, v => v.EndsWith(value));
                case TextFilter.Contain:
                    return GetNotInvertedBody(callBody, v => v.Contains(value));
                case TextFilter.NotContain:
                    return GetInvertedBody(callBody, v => !v.Contains(value));
                default:
                    throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
            }
        }

        static Expression GetNotInvertedBody(Expression callBody, Expression<Func<string, bool>> callback)
        {
            var body = (MethodCallExpression) callback.Body;
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            var concatExpressionBody = Expression.Call(callBody, containsMethodInfo, containsArgument);

            return concatExpressionBody;
        }

        static Expression GetInvertedBody(Expression callBody, Expression<Func<string, bool>> callback)
        {
            var body = (MethodCallExpression) ((UnaryExpression) callback.Body).Operand;
            var containsMethodInfo = body.Method;
            var containsArgument = body.Arguments[0];
            var concatExpressionBody = Expression.Not(Expression.Call(callBody, containsMethodInfo, containsArgument));

            return concatExpressionBody;
        }
    }
}