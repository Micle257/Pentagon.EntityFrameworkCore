﻿// -----------------------------------------------------------------------
//  <copyright file="QueryableExtensions.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Collections;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Specifications;

    public static class QueryableExtensions
    {
        static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");
        static readonly FieldInfo QueryModelGeneratorField = typeof(QueryCompiler).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryModelGenerator");
        static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");
        static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static string ToSql<TEntity>(this IQueryable<TEntity> query)
        {
            var queryCompiler = (QueryCompiler) QueryCompilerField.GetValue(query.Provider);
            var queryModelGenerator = (QueryModelGenerator) QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = queryModelGenerator.ParseQuery(query.Expression);
            var database = DataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies) DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor) queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.First().ToString();

            return sql;
        }

        public static Task<PagedList<TEntity>> ToPagedListAsync<TEntity>([NotNull] this IQueryable<TEntity> query, int pageNumber, int pageSize)
        {
            return ToPagedListAsync(query, new PaginationParameters {PageSize = pageSize, PageNumber = pageNumber});
        }

        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity>([NotNull] this IQueryable<TEntity> query, [NotNull] PaginationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.AreValid == false)
                throw new InvalidPaginationParametersException(parameters);

            var count = await query.CountAsync().ConfigureAwait(false);

            var possiblePageCount = count / parameters.PageSize + 1;

            if (parameters.PageNumber > possiblePageCount + 1)
                throw new ArgumentOutOfRangeException(nameof(parameters.PageNumber), message: "The page number is out of range.");
            
            query = query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);

            var list = await query.ToListAsync().ConfigureAwait(false);

            return new PagedList<TEntity>(list, count, parameters.PageSize, parameters.PageNumber - 1);
        }
        
        public static async Task<int> CountPagesAsync<TEntity>([NotNull] this IQueryable<TEntity> query, int pageSize)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (pageSize < 1)
                throw new InvalidPaginationParametersException(new PaginationParameters {  PageSize = pageSize});
            
            var count = await query.CountAsync().ConfigureAwait(false);

            return count / pageSize + 1;
        }

        public static async Task<int> CountPagesAsync<TEntity>([NotNull] this IQueryable<TEntity> query, PaginationParameters parameters)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (parameters?.AreValid == false)
                throw new InvalidPaginationParametersException(parameters);

            var count = await query.CountAsync().ConfigureAwait(false);

            return count / parameters.PageSize + 1;
        }
    }
}