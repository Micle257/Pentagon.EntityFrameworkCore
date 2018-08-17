// -----------------------------------------------------------------------
//  <copyright file="FilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;

    public static class TextFilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity>(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value)
        {
            var ex = GetFilterCallback(propertySelector.Body, filter, value);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            ex = ParameterReplacer.Replace(ex, parameter);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), ex, parameter);
        }
        
        public static Expression<Func<TEntity, bool>> GetDoubleFilter<TEntity>(Expression<Func<TEntity, string>> propertySelector,
                                                                                   TextFilter firstFilter,
                                                                                   string firstValue,
                                                                                   FilterLogicOperation operation,
                                                                                   TextFilter secondFilter,
                                                                                   string secondValue)
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

            expression = ParameterReplacer.Replace(expression, parameter);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), expression, parameter);
        }

        static Expression GetFilterCallback(Expression callBody, TextFilter textFilter, string value)
        {
            switch (textFilter)
            {
                case TextFilter.Equal:
                    return GetBody(callBody, v => v.Equals(value));
                case TextFilter.NotEqual:
                    return GetBody(callBody, v => !v.Equals(value));
                case TextFilter.Empty:
                    return GetBody(callBody, v => string.IsNullOrWhiteSpace(v));
                case TextFilter.NotEmpty:
                    return GetBody(callBody, v => !string.IsNullOrWhiteSpace(v));
                case TextFilter.StartWith:
                    return GetBody(callBody, v => v.StartsWith(value));
                case TextFilter.EndWith:
                    return GetBody(callBody, v => v.EndsWith(value));
                case TextFilter.Contain:
                    return GetBody(callBody, v => v.Contains(value));
                case TextFilter.NotContain:
                    return GetBody(callBody, v => !v.Contains(value));
                default:
                    throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
            }
        }

        static Expression GetBody(Expression callBody, Expression<Func<string, bool>> callback)
        {
            var parameter = callback.Parameters[0];
            var body = callback.Body;

          return  ParameterReplacer.Replace(body, parameter, callBody);
        }
    }
}