﻿// -----------------------------------------------------------------------
//  <copyright file="ValueFilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class ValueFilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector, IEnumerable<TProperty> values)
        {
            var ex = GetFilterCallback(propertySelector.Body, values);

            var parameter = Expression.Parameter(typeof(TEntity), name: "e");

            ex = ParameterReplacer.Replace(ex, parameter);

            return (Expression<Func<TEntity, bool>>) Expression.Lambda(typeof(Func<TEntity, bool>), ex, parameter);
        }

        static Expression GetFilterCallback<TProperty>(Expression callBody, IEnumerable<TProperty> values)
        {
            var callbacks = new List<Expression>();

            foreach (var value in values)
                callbacks.Add(GetBody<TProperty>(callBody, v => EqualityComparer<TProperty>.Default.Equals(v, value)));

            var conditionExpression = callbacks.Aggregate(Expression.OrElse);

            return conditionExpression;
        }

        static Expression GetBody<TProperty>(Expression callBody, Expression<Func<TProperty, bool>> callback)
        {
            var parameter = callback.Parameters[0];
            var body = callback.Body;

            return ParameterReplacer.Replace(body, parameter, callBody);
        }
    }
}