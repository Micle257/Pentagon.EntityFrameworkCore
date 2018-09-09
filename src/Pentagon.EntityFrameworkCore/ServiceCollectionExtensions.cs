// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Linq;
    using Abstractions;
    using Abstractions.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Repositories;
    using Synchronization;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseSynchronization<TRemoteContext, TLocalContext>(this IServiceCollection builder)
                where TRemoteContext : IApplicationContext
                where TLocalContext : IApplicationContext
        {
            builder.AddTransient<IDbContextSynchronizator, DbContextSynchronizator>();
            builder.AddTransient<ISynchronizationFactory, SynchronizationFactory<TRemoteContext, TLocalContext>>();
            builder.AddTransient<IRepositoryActionService, RepositoryActionService>();

            return builder;
        }

        /// <summary> Adds the unit of work services to <see cref="IServiceCollection" />. </summary>
        /// <typeparam name="TDbContext"> The type of the database context. </typeparam>
        /// <typeparam name="TContext"> The type of the context. </typeparam>
        /// <typeparam name="TDbContextFactoryImplementation"> The type of the factory for database and app contexts. </typeparam>
        /// <param name="builder"> The builder. </param>
        /// <returns> </returns>
        public static IServiceCollection AddUnitOfWork<TDbContext, TContext, TDbContextFactoryImplementation>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
                where TDbContextFactoryImplementation : class, IContextFactory<TContext>
        {
            builder.AddUnitOfWorkCore<TContext>(lifetime);

            builder.AddAppContextFromFactory<TContext, TDbContextFactoryImplementation>(lifetime)
                   .AddDbContextFromAppContext<TDbContext, TContext>(lifetime);

            return builder;
        }

        public static IServiceCollection AddUnitOfWork<TDbContext, TContext>(this IServiceCollection builder, Action<DbContextOptionsBuilder> configure = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.AddUnitOfWorkCore<TContext>(lifetime);

            builder.AddAppContextFromDbContext<TDbContext, TContext>(lifetime)
                   .AddDbContextFromBase<TDbContext, TContext>(configure, lifetime);

            return builder;
        }

        public static IServiceCollection AddUnitOfWorkWithPool<TDbContext, TContext>(this IServiceCollection builder,
                                                                                     Action<DbContextOptionsBuilder> configure = null,
                                                                                     ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.AddUnitOfWorkCore<TContext>(lifetime);

            builder.AddAppContextFromDbContext<TDbContext, TContext>(lifetime)
                   .AddDbContextFromBaseInPool<TDbContext, TContext>(configure);

            return builder;
        }

        public static IServiceCollection AddAppContextFromFactory<TContext, TFactoryImplementation>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TContext : class, IApplicationContext
                where TFactoryImplementation : class, IContextFactory<TContext>
        {
            builder.Add(new ServiceDescriptor(typeof(TContext), c => c.GetService<IContextFactory<TContext>>().CreateContext(), lifetime));

            builder.AddTransient<IContextFactory<TContext>, TFactoryImplementation>();

            return builder;
        }

        public static IServiceCollection AddAppContextFromDbContext<TDbContext, TContext>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.Add(new ServiceDescriptor(typeof(TContext), c => c.GetService<TDbContext>(), lifetime));

            return builder;
        }

        public static IServiceCollection AddDefaultAppContext<TContext>(this IServiceCollection builder)
                where TContext : class, IApplicationContext
        {
            var descriptor = builder.FirstOrDefault(a => a.ServiceType == typeof(TContext));

            if (descriptor != null)
                builder.Add(new ServiceDescriptor(typeof(IApplicationContext), c => c.GetService<TContext>(), descriptor.Lifetime));

            return builder;
        }

        /// <summary> Adds the database context (<see cref="DbContext" />) to service collection. The <see cref="DbContext" /> must implement <see cref="TContext" />. </summary>
        /// <typeparam name="TDbContext"> The type of the database context. </typeparam>
        /// <typeparam name="TContext"> The type of the context. </typeparam>
        /// <param name="builder"> The builder. </param>
        /// <returns> The service collection. </returns>
        public static IServiceCollection AddDbContextFromAppContext<TDbContext, TContext>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.Add(new ServiceDescriptor(typeof(TDbContext), c => c.GetService<TContext>() as TDbContext, lifetime));

            return builder;
        }

        public static IServiceCollection AddDbContextFromBase<TDbContext, TContext>(this IServiceCollection builder,
                                                                                    Action<DbContextOptionsBuilder> configure = null,
                                                                                    ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.AddDbContext<TDbContext>(configure, lifetime, lifetime);

            return builder;
        }

        public static IServiceCollection AddDbContextFromBaseInPool<TDbContext, TContext>(this IServiceCollection builder, Action<DbContextOptionsBuilder> configure = null)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.AddDbContextPool<TDbContext>(configure);

            return builder;
        }

        internal static IServiceCollection AddDefaultUnitOfWork<TContext>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TContext : class, IApplicationContext
        {
            builder.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory<IApplicationContext>>();
            builder.AddTransient<IUnitOfWorkFactory<IApplicationContext>, UnitOfWorkFactory<IApplicationContext>>();

            builder.AddTransient<IUnitOfWorkCommitExecutor, UnitOfWorkCommitExecutor<IApplicationContext>>();
            builder.AddTransient<IUnitOfWorkCommitExecutor<IApplicationContext>, UnitOfWorkCommitExecutor<IApplicationContext>>();

            builder.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork<IApplicationContext>), lifetime));
            builder.Add(new ServiceDescriptor(typeof(IUnitOfWork<IApplicationContext>), typeof(UnitOfWork<IApplicationContext>), lifetime));

            builder.AddTransient<IUnitOfWorkScope, UnitOfWorkScope<IApplicationContext>>();
            builder.AddTransient<IUnitOfWorkScope<IApplicationContext>, UnitOfWorkScope<IApplicationContext>>();

            return builder;
        }

        internal static IServiceCollection AddDbContextServices(this IServiceCollection builder)
        {
            builder.AddTransient<IDbContextDeleteService, DbContextDeleteService>();
            builder.AddTransient<IDbContextUpdateService, DbContextUpdateService>();

            return builder;
        }

        internal static IServiceCollection AddRepositoryFactory(this IServiceCollection builder)
        {
            builder.AddTransient<IRepositoryFactory, RepositoryFactory>();

            return builder;
        }

        internal static IServiceCollection AddPagination(this IServiceCollection builder)
        {
            builder.AddTransient<IPaginationService, PaginationService>();

            return builder;
        }

        internal static IServiceCollection AddCommitManager(this IServiceCollection builder, ServiceLifetime lifetime)
        {
            builder.AddSingleton<IDatabaseCommitManager, DatabaseCommitManager>();

            builder.Add(new ServiceDescriptor(typeof(IDatabaseCommitManager), typeof(DatabaseCommitManager), lifetime));

            return builder;
        }

        static IServiceCollection AddUnitOfWorkCore<TContext>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TContext : class, IApplicationContext
        {
            builder.AddLogging();

            // UoW
            builder.AddDbContextServices()
                   .AddRepositoryFactory()
                   .AddPagination()
                   .AddCommitManager(lifetime);

            builder.Add(new ServiceDescriptor(typeof(IDataUserProvider), typeof(DataUserProvider), lifetime));

            builder.AddTransient<IUnitOfWorkFactory<TContext>, UnitOfWorkFactory<TContext>>();
            builder.AddTransient<IConcurrencyConflictResolver<TContext>, ConcurrencyConflictResolver<TContext>>();
            builder.AddTransient<IUnitOfWorkCommitExecutor<TContext>, UnitOfWorkCommitExecutor<TContext>>();

            builder.Add(new ServiceDescriptor(typeof(IUnitOfWork<TContext>), typeof(UnitOfWork<TContext>), lifetime));
            builder.AddTransient<IUnitOfWorkScope<TContext>, UnitOfWorkScope<TContext>>();

            builder.AddDefaultUnitOfWork<TContext>();

            return builder;
        }
    }
}