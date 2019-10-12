// -----------------------------------------------------------------------
//  <copyright file="FilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Filters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Helpers;
    using JetBrains.Annotations;

    public static class FilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity, TValue>([NotNull] CompositeFilter<TEntity, TValue> filter)
        {
            if (filter.Type == 0)
                return null;

            IPredicateBuilder<TEntity> builder = new PredicateBuilder<TEntity>();

            var firstCallback = filter.FirstCondition;

            builder = (IPredicateBuilder<TEntity>)builder.Start((firstCallback));

            if (filter.Type == FilterCompositionType.Double)
            {
                var secondCallback = filter.SecondCondition;

                switch (filter.Operation)
                {
                    case FilterLogicOperation.Or:
                        builder = (IPredicateBuilder<TEntity>)builder.Or((secondCallback));
                        break;

                    case FilterLogicOperation.And:
                        builder = (IPredicateBuilder<TEntity>)builder.And((secondCallback));
                        break;
                }
            }

            var predicate = builder.Build();

            return predicate;
        }

        public static Expression<Func<TEntity, bool>> GetNumberFilterCallback<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertySelector, NumberFilter textFilter, TValue value)
        {
            var callBody = propertySelector.Body;

            return textFilter switch
            {
                NumberFilter.Equal => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => v.Equals(value))),
                NumberFilter.NotEqual => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => !v.Equals(value))),
                NumberFilter.GreatenThan => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer<TValue>.Default.Compare(v, value) > 0)),
                NumberFilter.GreatenThenOrEqualTo => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer<TValue>.Default.Compare(v, value) > 0 || v.Equals(value))),
                NumberFilter.LessThen => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer<TValue>.Default.Compare(v, value) < 0)),
                NumberFilter.LessThenOrEqualTo => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer<TValue>.Default.Compare(v, value) < 0 || v.Equals(value))),
                NumberFilter.Empty => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => v == null)),
                NumberFilter.NotEmpty => ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => v != null)),
                _ => throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null)
            };
        }

        public static Expression<Func<TEntity, bool>> GetTextFilterCallback<TEntity>([NotNull] Expression<Func<TEntity, string>> propertySelector, TextFilter textFilter, string value, StringComparison stringComparison)
        {
            var callBody = propertySelector.Body;

            return textFilter switch
            {
                TextFilter.Equal => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.Equals(value, stringComparison))),
                TextFilter.NotEqual => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !v.Equals(value, stringComparison))),
                TextFilter.Empty => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => string.IsNullOrWhiteSpace(v))),
                TextFilter.NotEmpty => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !string.IsNullOrWhiteSpace(v))),
                TextFilter.StartWith => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.StartsWith(value, stringComparison))),
                TextFilter.EndWith => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.EndsWith(value, stringComparison))),
                TextFilter.Contain => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.Contains(value, stringComparison))),
                TextFilter.NotContain => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !v.Contains(value, stringComparison))),
                _ => throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null)
            };
        }

        [NotNull]
        static Expression GetBody<TValue>([NotNull] Expression callBody, [NotNull] Expression<Func<TValue, bool>> callback)
        {
            var parameter = callback.Parameters[0];
            var body = callback.Body;

            return ExpressionParameterReplacer.Replace(body, parameter, callBody);
        }

        [NotNull]
        static Expression<Func<T, bool>> ConvertBodyToLambda<T>([NotNull] Expression body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Debug.Assert(body.Type == typeof(bool));

            var parameter = Expression.Parameter(typeof(T), "a");

            var fixedBody = ExpressionParameterReplacer.Replace(body, parameter);

            return Expression.Lambda<Func<T, bool>>(fixedBody, parameter);
        }

        [NotNull]
        public static Expression<Func<TEntity, object>> GetObjectPropertySelector<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> propertySelector)
        {
            var body = propertySelector.Body;

            var boxedBody = Expression.Convert(body, typeof(object));

            var parameter = Expression.Parameter(typeof(TEntity), "entity");

            var fixedBody = ExpressionParameterReplacer.Replace(boxedBody, parameter);

            return Expression.Lambda<Func<TEntity, object>>(fixedBody, parameter);
        }

        [NotNull]
        public static Expression<Func<TEntity, string>> GetStringPropertySelector<TEntity, TValue>([NotNull] Expression<Func<TEntity, TValue>> propertySelector)
        {
            var body = propertySelector.Body;

            var toStringCall = Expression.Call(body, body.Type.GetMethod(nameof(ToString), Array.Empty<Type>()));

            var parameter = Expression.Parameter(typeof(TEntity), "a");

            var fixedBody = ExpressionParameterReplacer.Replace(toStringCall, parameter);

            return Expression.Lambda<Func<TEntity, string>>(fixedBody, parameter);
        }
    }
}