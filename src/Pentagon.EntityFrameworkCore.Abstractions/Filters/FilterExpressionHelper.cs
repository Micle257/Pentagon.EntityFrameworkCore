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
    using Extensions;
    using Helpers;
    using JetBrains.Annotations;

    public static class FilterExpressionHelper
    {
        public static Expression<Func<TEntity, bool>> GetFilter<TEntity>([NotNull] ICompositeFilter filter)
        {
            if (filter.Type == 0)
                return null;

            IPredicateBuilder<TEntity> builder = new PredicateBuilder<TEntity>();

            if (filter is CompositeFilter<TEntity> basicCompositeFilter)
            {
                builder = BasicCompositeFilterSetupBuild(builder, basicCompositeFilter);
            }
            else if (filter.GetType().GenericTypeArguments?.Length == 2)
            {
                builder = CompositeFilterSetupBuild(builder, filter);
            }

            var predicate = builder.Build();

            return predicate;
        }

        [Pure]
        [NotNull]
        static IPredicateBuilder<TEntity> CompositeFilterSetupBuild<TEntity>([NotNull] IPredicateBuilder<TEntity> builder, [NotNull] ICompositeFilter filter)
        {
            var type = filter.GetType();

            var valueType = type.GenericTypeArguments[1];

            var selector = GetEntitySelector<TEntity>(filter);

            var firstConditionPropertyInfo = type.GetProperty(nameof(CompositeFilter<int, int>.FirstCondition));

            var firstCallback = (LambdaExpression)firstConditionPropertyInfo.GetValue(filter);

            var firstCallbackBody = firstCallback.Body;

            var result = firstCallbackBody;

            if (filter.Type == FilterCompositionType.Double)
            {
                var secondConditionPropertyInfo = type.GetProperty(nameof(CompositeFilter<int, int>.SecondCondition));

                var secondCallback = (LambdaExpression)secondConditionPropertyInfo.GetValue(filter);

                var secondCallbackBody = secondCallback.Body;

                switch (filter.Operation)
                {
                    case FilterLogicOperation.Or:
                        result = Expression.OrElse(firstCallbackBody, secondCallbackBody);
                        break;

                    case FilterLogicOperation.And:
                        result = Expression.AndAlso(firstCallbackBody, secondCallbackBody);
                        break;
                }
            }

            builder.Start();
        }

        static Expression<Func<TEntity>> GetEntitySelector<TEntity>(ICompositeFilter filter)
        {

            var firstConditionPropertyInfo = type.GetProperty(nameof(CompositeFilter<int, int>.S));
        }

        [Pure]
        [NotNull]
        static IPredicateBuilder<TEntity> BasicCompositeFilterSetupBuild<TEntity>([NotNull] IPredicateBuilder<TEntity> builder, [NotNull] CompositeFilter<TEntity> filter)
        {
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

            return builder;
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

        public static Expression<Func<TEntity, bool>> GetTextFilterCallback<TEntity>([NotNull] Expression<Func<TEntity, string>> propertySelector, TextFilter textFilter, string value)
        {
            var callBody = propertySelector.Body;

            return textFilter switch
            {
                TextFilter.Equal => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.Equals(value))),
                TextFilter.NotEqual => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !v.Equals(value))),
                TextFilter.Empty => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => string.IsNullOrWhiteSpace(v))),
                TextFilter.NotEmpty => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !string.IsNullOrWhiteSpace(v))),
                TextFilter.StartWith => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.StartsWith(value))),
                TextFilter.EndWith => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.EndsWith(value))),
                TextFilter.Contain => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => v.Contains(value))),
                TextFilter.NotContain => ConvertBodyToLambda<TEntity>(GetBody<string>(callBody, v => !v.Contains(value))),
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

            if (body is UnaryExpression ue && body.NodeType == ExpressionType.Convert)
            {
                if (ue.Operand.Type == typeof(string))
                {
                    body = ue.Operand;
                }
            }
            else
            {
                body = Expression.Call(body, body.Type.GetMethod(nameof(ToString), Array.Empty<Type>()));
            }

            var parameter = Expression.Parameter(typeof(TEntity), "a");

            var fixedBody = ExpressionParameterReplacer.Replace(body, parameter);

            return Expression.Lambda<Func<TEntity, string>>(fixedBody, parameter);
        }

        [NotNull]
        public static Expression<Func<TEntity, bool>> FlattenComposedFilter<TEntity, TProperty>([NotNull] CompositeFilter<TEntity, TProperty> filter)
        {
            var first = filter.FirstCondition;
            var second = filter.SecondCondition;
            var select = filter.Property;

            var firstBody  = first.Body;
            var secondBody = second.Body;
            var selectBody = select.Body;


        }
    }
}