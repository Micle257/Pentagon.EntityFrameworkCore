// -----------------------------------------------------------------------
//  <copyright file="FilterExpressionHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications.Filters
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using Abstractions.Entities;
    using Helpers;

    public static class FilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity, TValue>(CompositeFilter<TEntity, TValue> filter)
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

       //public static Expression<Func<TEntity, bool>> GetFilter<TEntity, TFilter, TValue>(CompositeFilter<TEntity, TFilter, TValue> filter)
       //        where TFilter : struct, Enum
       //{
       //    if (filter.Type == 0)
       //        return null;
       //
       //    IPredicateBuilder<TEntity> builder = new PredicateBuilder<TEntity>();
       //
       //    Expression firstCallback = null;
       //
       //    switch (filter.FirstCondition)
       //    {
       //        case NumberFilter nf:
       //            firstCallback = GetNumberFilterCallback(filter.Property.Body, nf, filter.FirstValue);
       //            break;
       //
       //        case TextFilter tf:
       //            firstCallback = GetTextFilterCallback(filter.Property.Body, tf, filter.FirstValue as string);
       //            break;
       //    }
       //
       //    builder = (IPredicateBuilder<TEntity>) builder.Start(ConvertBodyToLambda<TEntity>(firstCallback));
       //
       //    if (filter.Type == FilterCompositionType.Double)
       //    {
       //        Expression secondCallback = null;
       //
       //        switch (filter.SecondCondition)
       //        {
       //            case NumberFilter nf:
       //                secondCallback = GetNumberFilterCallback(filter.Property.Body, nf, filter.SecondValue);
       //                break;
       //
       //            case TextFilter tf:
       //                secondCallback = GetTextFilterCallback(filter.Property.Body, tf, filter.SecondValue as string);
       //                break;
       //        }
       //
       //        switch (filter.Operation)
       //        {
       //            case FilterLogicOperation.Or:
       //                builder = (IPredicateBuilder<TEntity>) builder.Or(ConvertBodyToLambda<TEntity>(secondCallback));
       //                break;
       //
       //            case FilterLogicOperation.And:
       //                builder = (IPredicateBuilder<TEntity>) builder.And(ConvertBodyToLambda<TEntity>(secondCallback));
       //                break;
       //        }
       //    }
       //
       //    var predicate = builder.Build();
       //
       //    return predicate;
       //}

       public static Expression<Func<TEntity, bool>> GetNumberFilterCallback<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertySelector, NumberFilter textFilter, TValue value)
        {
            var callBody = propertySelector.Body;

            switch (textFilter)
            {
                case NumberFilter.Equal:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => v.Equals(value)));
                case NumberFilter.NotEqual:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => !v.Equals(value)));
                case NumberFilter.GreatenThan:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer.Default.Compare(v, value) > 0));
                case NumberFilter.GreatenThenOrEqualTo:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer.Default.Compare(v, value) > 0 || v.Equals(value)));
                case NumberFilter.LessThen:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer.Default.Compare(v, value) < 0));
                case NumberFilter.LessThenOrEqualTo:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => Comparer.Default.Compare(v, value) < 0 || v.Equals(value)));
            }

            switch (textFilter)
            {
                case NumberFilter.Empty:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => v == null));
                case NumberFilter.NotEmpty:
                    return ConvertBodyToLambda<TEntity>(GetBody<TValue>(callBody, v => v != null));
            }

            throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
        }

        public static Expression<Func<TEntity, bool>> GetTextFilterCallback<TEntity>(Expression<Func<TEntity, string>> propertySelector, TextFilter textFilter, string value)
        {
            var callBody = propertySelector.Body;

            switch (textFilter)
            {
                case TextFilter.Equal:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.Equals(value)));
                case TextFilter.NotEqual:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !v.Equals(value)));
                case TextFilter.Empty:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => string.IsNullOrWhiteSpace(v)));
                case TextFilter.NotEmpty:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !string.IsNullOrWhiteSpace(v)));
                case TextFilter.StartWith:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.StartsWith(value)));
                case TextFilter.EndWith:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.EndsWith(value)));
                case TextFilter.Contain:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.Contains(value)));
                case TextFilter.NotContain:
                    return ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !v.Contains(value)));
                default:
                    throw new ArgumentOutOfRangeException(nameof(textFilter), textFilter, null);
            }
        }

        static Expression GetBody<TValue>(Expression callBody, Expression<Func<TValue, bool>> callback)
        {
            var parameter = callback.Parameters[0];
            var body = callback.Body;

            return ExpressionParameterReplacer.Replace(body, parameter, callBody);
        }

        static Expression<Func<T, bool>> ConvertBodyToLambda<T>(Expression body)
        {
            var parameter = Expression.Parameter(typeof(T), "a");

            var fixedBody = ExpressionParameterReplacer.Replace(body, parameter);

            return Expression.Lambda<Func<T, bool>>(fixedBody, parameter);
        }

        public static Expression<Func<TEntity, object>> GetObjectPropertySelector<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertySelector)
        {
            var body = propertySelector.Body;

            var boxedBody = Expression.Convert(body, typeof(object));

            var parameter = Expression.Parameter(typeof(TEntity), "entity");

            var fixedBody = ExpressionParameterReplacer.Replace(boxedBody, parameter);

            return Expression.Lambda<Func<TEntity, object>>(fixedBody, parameter);
        }

        public static Expression<Func<TEntity, string>> GetStringPropertySelector<TEntity, TValue>(Expression<Func<TEntity, TValue>> propertySelector)
        {
            var body = propertySelector.Body;

            var toStringCall = Expression.Call(body, body.Type.GetMethod(nameof(ToString), Array.Empty<Type>()));

            var parameter = Expression.Parameter(typeof(TEntity), "a");

            var fixedBody = ExpressionParameterReplacer.Replace(toStringCall, parameter);

            return Expression.Lambda<Func<TEntity, string>>(fixedBody, parameter);
        }
    }
}