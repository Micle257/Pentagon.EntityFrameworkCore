// -----------------------------------------------------------------------
//  <copyright file="FilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using Helpers;

    public static class FilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity, TFilter, TValue>(CompositeFilter<TEntity, TFilter, TValue> filter)
                where TFilter : struct, Enum
        {
            if (filter.Type == 0)
                return null;

            IPredicateBuilder<TEntity> builder = new PredicateBuilder<TEntity>();

            Expression firstCallback = null;

            switch (filter.FirstCondition)
            {
                case NumberFilter nf:
                    firstCallback = GetNumberFilterCallback(filter.Property, nf, filter.FirstValue);
                    break;

                case TextFilter tf:
                    firstCallback = GetTextFilterCallback(filter.Property, tf, filter.FirstValue as string);
                    break;
            }

            builder = (IPredicateBuilder<TEntity>) builder.Start(ConvertBodyToLambda<TEntity>(firstCallback));

            if (filter.Type == FilterCompositionType.Double)
            {
                Expression secondCallback = null;

                switch (filter.SecondCondition)
                {
                    case NumberFilter nf:
                        secondCallback = GetNumberFilterCallback(filter.Property, nf, filter.FirstValue);
                        break;

                    case TextFilter tf:
                        secondCallback = GetTextFilterCallback(filter.Property, tf, filter.FirstValue as string);
                        break;
                }

                switch (filter.Operation)
                {
                    case FilterLogicOperation.Or:
                        builder = (IPredicateBuilder<TEntity>) builder.Or(ConvertBodyToLambda<TEntity>(secondCallback));
                        break;

                    case FilterLogicOperation.And:
                        builder = (IPredicateBuilder<TEntity>) builder.And(ConvertBodyToLambda<TEntity>(secondCallback));
                        break;
                }
            }

            var predicate = builder.Build();

            return predicate;
        }

        static Expression<Func<T, bool>> ConvertBodyToLambda<T>(Expression body)
        {
            var parameter = Expression.Parameter(typeof(T));

            var fixedBody = ParameterReplacer.Replace(body, parameter);

            return Expression.Lambda<Func<T, bool>>(fixedBody, parameter);
        }

        static Expression GetNumberFilterCallback(Expression callBody, NumberFilter textFilter, object value)
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

        static Expression GetTextFilterCallback(Expression callBody, TextFilter textFilter, string value)
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

            return ParameterReplacer.Replace(body, parameter, callBody);
        }
    }
}