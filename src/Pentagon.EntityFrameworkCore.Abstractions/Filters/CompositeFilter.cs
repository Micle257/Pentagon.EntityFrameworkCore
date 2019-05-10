// -----------------------------------------------------------------------
//  <copyright file="CompositeFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Specifications.Filters
{
    using System;
    using System.Linq.Expressions;

    public class CompositeFilter<TEntity, TFilter, TValue>
        where TFilter : struct, Enum
    {
        public Guid Id { get; set; }

        public Expression<Func<TEntity, TValue>> Property { get; set; }

        public TFilter? FirstCondition { get; set; }

        public TValue FirstValue { get; set; }

        public FilterLogicOperation Operation { get; set; }

        public TFilter? SecondCondition { get; set; }

        public TValue SecondValue { get; set; }

        public FilterCompositionType Type
        {
            get
            {
                var valid = Property != null && FirstCondition != null;

                if (!valid)
                    return 0;

                return Operation != 0 && SecondCondition != null ? FilterCompositionType.Double : FilterCompositionType.Single;
            }
        }
    }

    public class CompositeFilter<TEntity, TValue>
    {
        public Guid Id { get; set; }

        public Expression<Func<TEntity, TValue>> Property { get; set; }

        public Expression<Func<TEntity, bool>> FirstCondition { get; set; }

        public TValue FirstValue { get; set; }

        public FilterLogicOperation Operation { get; set; }

        public Expression<Func<TEntity, bool>> SecondCondition { get; set; }

        public TValue SecondValue { get; set; }

        public FilterCompositionType Type
        {
            get
            {
                var valid = Property != null && FirstCondition != null;

                if (!valid)
                    return 0;

                return Operation != 0 && SecondCondition != null ? FilterCompositionType.Double : FilterCompositionType.Single;
            }
        }
    }
}