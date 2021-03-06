﻿// -----------------------------------------------------------------------
//  <copyright file="EntityHelper.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Interfaces.Entities;

    public static class EntityHelper
    {
        public static IEnumerable<PropertyInfo> GetPureProperties(object entity)
        {
            var type = entity.GetType();

            var typesToCheck = new[]
                               {
                                       typeof(IConcurrencyStampSupport),
                                       typeof(ICreateTimeStampSupport),
                                       typeof(ICreateStampSupport),
                                       typeof(IDeletedFlagSupport),
                                       typeof(IDeleteTimeStampIdentitySupport),
                                       typeof(IUpdateTimeStampIdentitySupport),
                                       typeof(ICreateTimeStampIdentitySupport),
                                       typeof(IEntity),
                                       typeof(IUpdateTimeStampSupport),
                                       typeof(IDeleteTimeStampSupport),
                                       typeof(ICreatedUserEntitySupport),
                                       typeof(IUpdatedUserEntitySupport),
                                       typeof(IDeletedUserEntitySupport)
                               };

            var props = typesToCheck.SelectMany(a => a.GetProperties().Select(b => b.Name))
                                    .Distinct();

            var except = type.GetProperties().Select(a => a.Name).Except(props);

            return except.Select(a => type.GetProperties().FirstOrDefault(b => b.Name == a));
        }
    }
}