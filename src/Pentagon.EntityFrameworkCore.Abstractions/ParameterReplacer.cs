// -----------------------------------------------------------------------
//  <copyright file="ParameterReplacer.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Linq.Expressions;

    public static class ParameterReplacer
    {
        public static Expression Replace(Expression expression,
                                         ParameterExpression source,
                                         Expression target) => new ParameterReplacerVisitor(source, target).Visit(expression);

        public static Expression Replace(Expression expression,
                                         ParameterExpression source) => new SimpleParameterReplacerVisitor(source).Visit(expression);

        class ParameterReplacerVisitor : ExpressionVisitor
        {
            readonly ParameterExpression _source;
            readonly Expression _target;

            public ParameterReplacerVisitor(ParameterExpression source, Expression target)
            {
                _source = source;
                _target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node) => node == _source ? _target : base.VisitParameter(node);
        }

        class SimpleParameterReplacerVisitor : ExpressionVisitor
        {
            readonly ParameterExpression _source;

            public SimpleParameterReplacerVisitor(ParameterExpression source)
            {
                _source = source;
            }

            protected override Expression VisitParameter(ParameterExpression node) => base.VisitParameter(_source);
        }
    }
}