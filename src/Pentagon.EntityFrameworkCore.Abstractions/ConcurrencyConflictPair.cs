// -----------------------------------------------------------------------
//  <copyright file="ConcurrencyConflictPair.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ConcurrencyConflictPair
    {
        public ConcurrencyConflictEntity Local { get; set; }

        public ConcurrencyConflictEntity Remote { get; set; }

        public bool IsValid
        {
            get
            {
                if (Local?.Entity == null || Remote?.Entity == null)
                    return false;

                return Local.Entity.GetType() == Remote.Entity.GetType();
            }
        }

        public IReadOnlyList<ConflictPairDifference> GetDifference()
        {
            if (!IsValid)
                return new List<ConflictPairDifference>();

            var local = Local.Entity;
            var remote = Remote.Entity;

            var props = EntityHelper.GetPureProperties(local)
                            .Where(a => a.PropertyType.IsPrimitive || (Nullable.GetUnderlyingType(a.PropertyType)?.IsPrimitive ?? false)
                                                                   || IsValueType<Guid>(a.PropertyType)
                                                                   || IsValueType<DateTimeOffset>(a.PropertyType)
                                                                   || IsValueType<DateTime>(a.PropertyType)
                                                                   || a.PropertyType == typeof(string));

            var res = new List<ConflictPairDifference>();

            foreach (var propertyInfo in props)
            {
                var l = propertyInfo.GetValue(local);
                var r = propertyInfo.GetValue(remote);

                var equal = Comparer.Default.Compare(l, r) == 0;

                if (!equal)
                {
                    res.Add(new ConflictPairDifference
                            {
                                    LocalValue = l,
                                    RemoteValue = r,
                                    PropertyInfo = propertyInfo,
                                    PropertyName = propertyInfo.Name
                            });
                }
            }

            return res;

            bool IsValueType<T>(Type typess) => typess == typeof(T) || Nullable.GetUnderlyingType(typess) == typeof(T);
        }
    }
}