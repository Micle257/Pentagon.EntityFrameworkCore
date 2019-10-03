// -----------------------------------------------------------------------
//  <copyright file="CompositeFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Filters
{
    using System;
    using System.Linq.Expressions;

    public class CompositeFilter<TEntity, TValue>
    {
        public Guid Id { get; set; }

        public Expression<Func<TEntity, bool>> FirstCondition { get; set; }

        public FilterLogicOperation Operation { get; set; }

        public Expression<Func<TEntity, bool>> SecondCondition { get; set; }

        public FilterCompositionType Type
        {
            get
            {
                if (FirstCondition == null)
                    return 0;

                return Operation != 0 && SecondCondition != null ? FilterCompositionType.Double : FilterCompositionType.Single;
            }
        }

        public Expression<Func<TEntity, TValue>> Property { get; set; }
    }
}