﻿namespace Pentagon.Data.EntityFramework
{
    using System;
    using Abstractions;
    using Abstractions.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Pentagon.Extensions.DependencyInjection;
    using Repositories;
    using Synchonization;

    public static class IServiceCollectionExtensions
    {
        internal static IServiceCollection AddDbContextServices(this IServiceCollection builder)
        {
            builder.AddSingleton<IDbContextDeleteService, DbContextDeleteService>();
            builder.AddSingleton<IDbContextIdentityService, DbContextIdentityService>();
            builder.AddSingleton<IDbContextUpdateService, DbContextUpdateService>();

            return builder;
        }

        public static IServiceCollection AddDatabaseSynchronization(this IServiceCollection builder)
        {
            builder.AddTransient<IDbContextSynchronizator, DbContextSynchronizator>();
            builder.AddTransient<ISynchronizationFactory, SynchronizationFactory>();
            builder.AddTransient<IRepositoryActionService, RepositoryActionService>();

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

        internal static IServiceCollection AddCommitManager(this IServiceCollection builder)
        {
            builder.AddSingleton<IDatabaseCommitManager, DatabaseCommitManager>();

            return builder;
        }

        public static IServiceCollection AddUnitOfWork<TDbContext, TContext>(this IServiceCollection builder)
                where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            // UoW
            builder.AddDbContext<TDbContext, TContext>()
                   .AddDbContextServices()
                   .AddRepositoryFactory()
                   .AddPagination()
                   .AddCommitManager();
            
            builder.AddTransient<IUnitOfWork<TContext>, UnitOfWork<TContext>>();

            builder.AddTransient<IUnitOfWorkFactory<TContext>, UnitOfWorkFactory<TContext>>();

            return builder;
        }

        public static IServiceCollection AddContext<TContext, TFactoryImplementation>(this IServiceCollection builder)
                where TContext : class, IApplicationContext
                where TFactoryImplementation : class, IContextFactory<TContext>
        {
            builder.AddContext<TContext>();

            builder.AddSingleton<IContextFactory<TContext>, TFactoryImplementation>();

            return builder;
        }
        
        public static IServiceCollection AddContext<TContext>(this IServiceCollection builder)
                where TContext : class, IApplicationContext
        {
            builder.AddTransient<TContext>(c => c.GetService<IContextFactory<TContext>>().CreateContext());

            return builder;
        }

        public static IServiceCollection AddDbContext<TDbContext, TContext>(this IServiceCollection builder)
            where TDbContext : DbContext, TContext
                where TContext : class, IApplicationContext
        {
            builder.AddTransient<TDbContext>(c => c.GetService<TContext>() as TDbContext);

            return builder;
        }
    }
}