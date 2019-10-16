// -----------------------------------------------------------------------
//  <copyright file="CompositeFilter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Filters
{
    using System;
    using System.Linq.Expressions;

    public interface ICompositeFilter {
        Guid Id { get;  }

        FilterLogicOperation Operation { get;  }

        FilterCompositionType Type { get; }
    }

    public class CompositeFilter<TEntity, TValue> : ICompositeFilter
    {
        public Guid Id { get; set; }

        public Expression<Func<TValue, bool>> FirstCondition { get; set; }

        public FilterLogicOperation Operation { get; set; }

        public Expression<Func<TValue, bool>> SecondCondition { get; set; }

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

    public class CompositeFilter<TEntity> : ICompositeFilter
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

        public Expression<Func<TEntity, object>> Property { get; set; }
    }
}