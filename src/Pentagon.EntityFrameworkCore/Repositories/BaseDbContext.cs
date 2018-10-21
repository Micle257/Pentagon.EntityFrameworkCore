﻿// -----------------------------------------------------------------------
//  <copyright file="BaseDbContext.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Repositories
{
    using System;
    using System.Linq;
    using Abstractions;
    using Abstractions.Entities;
    using Abstractions.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public abstract class BaseDbContext : DbContext, IApplicationContext
    {
        public BaseDbContext()
        {
            ChangeTracker.StateChanged += OnStateChanged;
            ChangeTracker.Tracked += OnTracked;
        }

        /// <inheritdoc />
        public event EventHandler<CommitEventArgs> Commiting;

        /// <inheritdoc />
        public bool UseTimeSourceFromEntities { get; set; }

        /// <inheritdoc />
        public IRepository<TEntity> GetRepository<TEntity>()
                where TEntity : class, IEntity, new() => new Repository<TEntity>(this);

        void OnTracked(object sender, EntityTrackedEventArgs args)
        {
            var entity = args.Entry.Entity as IEntity;

            // entity has been tracked (get, add ...), commited is like added (for UI change)
            OnCommiting(new CommitEventArgs(new Entry(entity, EntityStateType.Added)));
        }

        void OnStateChanged(object sender, EntityStateChangedEventArgs args)
        {
            var entity = args.Entry.Entity as IEntity;
            var state = args.NewState.ToEntityStateType();
            var oldState = args.OldState;

            // if state has not changed for the API, cancel
            if (state == 0)
                return;

            // if the change was from unchanged to added
            // cancel call, because it is adding entity scenario and it would end up in two event fires
            if (oldState == EntityState.Unchanged && state == EntityStateType.Added)
                return;

            OnCommiting(new CommitEventArgs(new Entry(entity, state)));
        }

        void OnCommiting(CommitEventArgs commitEventArgs)
        {
            Commiting?.Invoke(this, new CommitEventArgs(commitEventArgs?.Entries.ToArray()));
        }
    }
}