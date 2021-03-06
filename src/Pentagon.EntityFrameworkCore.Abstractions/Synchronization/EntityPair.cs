// -----------------------------------------------------------------------
//  <copyright file="EntityPair.cs">
//   Copyright (c) Michal Pokorn�. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Synchronization
{
    using Interfaces.Entities;

    public struct EntityPair<TEntity>
            where TEntity : class, IEntity
    {
        public EntityPair(TEntity local, TEntity remote)
        {
            Local = local;
            Remote = remote;
        }

        public TEntity Local { get; }

        public TEntity Remote { get; }

        public EntityPairType Type
        {
            get
            {
                if (Local == null && Remote != null)
                    return EntityPairType.RemoteOnly;
                if (Local != null && Remote == null)
                    return EntityPairType.LocalOnly;
                if (Local != null && Remote != null)
                    return EntityPairType.Both;

                return EntityPairType.Unspecified;
            }
        }
    }
}