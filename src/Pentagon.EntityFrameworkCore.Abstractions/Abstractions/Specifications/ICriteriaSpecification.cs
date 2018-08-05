// -----------------------------------------------------------------------
//  <copyright file="ICriteriaSpecification.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Abstractions.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Entities;
    using EntityFrameworkCore.Specifications;

    /// <summary> Represents an entity specification for query, adding filter condition for returned entities (WHERE in T-SQL). </summary>
    /// <typeparam name="TEntity"> The type of the entity. </typeparam>
    public interface IFilterSpecification<TEntity> : ISpecification<TEntity>
            where TEntity : IEntity
    {
        /// <summary> Gets the filter function collection. </summary>
        /// <value> The list of filters. </value>
        ICollection<Expression<Func<TEntity, bool>>> Filters { get; }

        IFilterSpecification<TEntity> AddTextFilter(Expression<Func<TEntity, string>> propertySelector, TextFilter filter, string value);

        IFilterSpecification<TEntity> AddTextDoubleFilter(Expression<Func<TEntity, string>> propertySelector,
                                                          TextFilter firstFilter,
                                                          string firstValue,
                                                          FilterLogicOperation operation,
                                                          TextFilter secondFilter,
                                                          string secondValue);

        IFilterSpecification<TEntity> AddNumberFilter(Expression<Func<TEntity, decimal>> propertySelector, NumberFilter filter, decimal value);

        IFilterSpecification<TEntity> AddNumberDoubleFilter(Expression<Func<TEntity, decimal>> propertySelector,
                                                            NumberFilter firstFilter,
                                                            decimal firstValue,
                                                            FilterLogicOperation operation,
                                                            NumberFilter secondFilter,
                                                            decimal secondValue);

        IFilterSpecification<TEntity> AddNumberFilter(Expression<Func<TEntity, int>> propertySelector, NumberFilter filter, int value);

        IFilterSpecification<TEntity> AddNumberDoubleFilter(Expression<Func<TEntity, int>> propertySelector,
                                                            NumberFilter firstFilter,
                                                            int firstValue,
                                                            FilterLogicOperation operation,
                                                            NumberFilter secondFilter,
                                                            int secondValue);
    }
}