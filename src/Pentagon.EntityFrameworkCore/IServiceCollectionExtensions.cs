// -----------------------------------------------------------------------
//  <copyright file="IServiceCollectionExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Abstractions;
    using Abstractions.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Repositories;
    using Synchronization;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseSynchronization(this IServiceCollection builder)
        {
            builder.AddTransient<IDbContextSynchronizator, DbContextSynchronizator>();
            builder.AddTransient<ISynchronizationFactory, SynchronizationFactory>();
            builder.AddTransient<IRepositoryActionService, RepositoryActionService>();

            return builder;
        }

        /// <summary>
        /// Adds the unit of work services to <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TDbContextFactoryImplementation">The type of the factory for database and app contexts. </typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWork<TDbContext, TContext, TDbContextFactoryImplementation>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
                where TDbContextFactoryImplementation : class, IContextFactory<TContext>
        {
            builder.AddLogging();

            // UoW
            builder.AddAppContext<TContext, TDbContextFactoryImplementation>(lifetime)
                    .AddAppDbContext<TDbContext, TContext>(lifetime)
                   .AddDbContextServices()
                   .AddRepositoryFactory()
                   .AddPagination()
                   .AddCommitManager(lifetime);
            
            builder.AddTransient<IUnitOfWorkFactory<TContext>, UnitOfWorkFactory<TContext>>();
            builder.AddTransient<IConcurrencyConflictResolver<TContext>, ConcurrencyConflictResolver<TContext>>();
            
            builder.Add(new ServiceDescriptor(typeof(IUnitOfWork<TContext>), typeof(UnitOfWork<TContext>), lifetime));
            builder.AddTransient<IUnitOfWorkScope<TContext>, UnitOfWorkScope<TContext>>();

            return builder;
        }

        public static IServiceCollection AddAppContext<TContext, TFactoryImplementation>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TContext : class, IApplicationContext
                where TFactoryImplementation : class, IContextFactory<TContext>
        {
            builder.AddAppContext<TContext>(lifetime);

            builder.AddTransient<IContextFactory<TContext>, TFactoryImplementation>();

            return builder;
        }

        public static IServiceCollection AddAppContext<TContext>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TContext : class, IApplicationContext
        {
            builder.AddTransient(c => c.GetService<IContextFactory<TContext>>().CreateContext());
            builder.Add(new ServiceDescriptor(typeof(TContext), c => c.GetService<IContextFactory<TContext>>().CreateContext(), lifetime));

            return builder;
        }

        /// <summary>
        /// Adds the database context (<see cref="DbContext"/>) to service collection. The <see cref="DbContext"/> must implement <see cref="TContext"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAppDbContext<TDbContext, TContext>(this IServiceCollection builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.Add(new ServiceDescriptor(typeof(TDbContext), c => c.GetService<TContext>() as TDbContext, lifetime));

            return builder;
        }

        internal static IServiceCollection AddDbContextServices(this IServiceCollection builder)
        {
            builder.AddTransient<IDbContextDeleteService, DbContextDeleteService>();
            builder.AddTransient<IDbContextIdentityService, DbContextIdentityService>();
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
    }
}