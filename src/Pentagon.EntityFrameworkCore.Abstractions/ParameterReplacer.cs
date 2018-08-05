// -----------------------------------------------------------------------
//  <copyright file="ParameterReplacer.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Linq.Expressions;

    public class ParameterReplacer : ExpressionVisitor
    {
        readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node) => base.VisitParameter(_parameter);
    }
}