﻿// -----------------------------------------------------------------------
//  <copyright file="NumberFilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class NumberFilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity>(Expression<Func<TEntity, object>> propertySelector, NumberFilter filter, object value)
        {
            var ex = GetFilterCallback(propertySelector.Body, filter, value);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            ex = ParameterReplacer.Replace(ex, parameter);

            return (Expression<Func<TEntity, bool>>)Expression.Lambda(typeof(Func<TEntity, bool>), ex, parameter);
        }

        public static Expression<Func<TEntity, bool>> GetDoubleFilter<TEntity>(Expression<Func<TEntity, object>> propertySelector,
                                                                               NumberFilter firstFilter,
                                                                               object firstValue,
                                                                               FilterLogicOperation operation,
                                                                               NumberFilter secondFilter,
                                                                               object secondValue)
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

            return (Expression<Func<TEntity, bool>>)Expression.Lambda(typeof(Func<TEntity, bool>), expression, parameter);
        }

        static Expression GetFilterCallback(Expression callBody, NumberFilter textFilter, object value)
        {
            switch (textFilter)
            {
                case NumberFilter.Equal:
                    return GetBody(callBody, v => v.Equals(value));
                case NumberFilter.NotEqual:
                    return GetBody(callBody, v => !v.Equals(value));
                case NumberFilter.GreatenThan:
                    return GetBody(callBody, v => Comparer.Default.Compare(v, value) > 0);
                case NumberFilter.GreatenThenOrEqualTo:
                    return GetBody(callBody, v => Comparer.Default.Compare(v, value) > 0 || v.Equals(value));
                case NumberFilter.LessThen:
                    return GetBody(callBody, v => Comparer.Default.Compare(v, value) < 0);
                case NumberFilter.LessThenOrEqualTo:
                    return GetBody(callBody, v => Comparer.Default.Compare(v, value) < 0 || v.Equals(value));
            }
            
                switch (textFilter)
                {
                    case NumberFilter.Empty:
                        return GetBody(callBody, v => v == null);
                    case NumberFilter.NotEmpty:
                        return GetBody(callBody, v => v != null);
                }

            throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
        }

        static Expression GetBody(Expression callBody, Expression<Func<object, bool>> callback)
        {
            var parameter = callback.Parameters[0];
            var body = callback.Body;

            return ParameterReplacer.Replace(body, parameter, callBody);
        }
    }

    public static class NumberFilterExpressionHelper<T>
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity>(Expression<Func<TEntity, T>> propertySelector, NumberFilter filter, T value)
        {
            var ex = GetFilterCallback(propertySelector.Body, filter, value);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            ex = ParameterReplacer.Replace(ex, parameter);

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

            expression = ParameterReplacer.Replace(expression, parameter);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), expression, parameter);
        }

        static Expression GetFilterCallback(Expression callBody, NumberFilter textFilter, T value)
        {
            switch (textFilter)
            {
                case NumberFilter.Equal:
                    return GetBody(callBody, v => v.Equals(value));
                case NumberFilter.NotEqual:
                    return GetBody(callBody, v => !v.Equals(value));
                case NumberFilter.GreatenThan:
                    return GetBody(callBody, v => Comparer<T>.Default.Compare(v, value) > 0);
                case NumberFilter.GreatenThenOrEqualTo:
                    return GetBody(callBody, v => Comparer<T>.Default.Compare(v, value) > 0 || v.Equals(value));
                case NumberFilter.LessThen:
                    return GetBody(callBody, v => Comparer<T>.Default.Compare(v, value) < 0);
                case NumberFilter.LessThenOrEqualTo:
                    return GetBody(callBody, v => Comparer<T>.Default.Compare(v, value) < 0 || v.Equals(value));
            }
            
                switch (textFilter)
                {
                    case NumberFilter.Empty:
                        return GetBody(callBody, v => v == null);
                    case NumberFilter.NotEmpty:
                        return GetBody(callBody, v => v != null);
                }

            throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
        }

        static Expression GetBody(Expression callBody, Expression<Func<T, bool>> callback)
        {
            var parameter = callback.Parameters[0];
            var body = callback.Body;

            return  ParameterReplacer.Replace(body, parameter, callBody);
        }
    }
}