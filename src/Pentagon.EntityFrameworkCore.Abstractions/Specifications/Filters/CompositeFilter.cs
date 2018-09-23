// -----------------------------------------------------------------------
//  <copyright file="CompositeFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications
{
    using System;
    using System.Linq.Expressions;

    public class CompositeFilter<TEntity, TFilter, TValue>
    {
        public Expression<Func<TEntity, TValue>> Property { get; set; }

        public TFilter FirstCondition { get; set; }

        public TValue FirstValue { get; set; }

        public FilterLogicOperation Operation { get; set; }

        public TFilter SecondCondition { get; set; }

        public TValue SecondValue { get; set; }
    }
}