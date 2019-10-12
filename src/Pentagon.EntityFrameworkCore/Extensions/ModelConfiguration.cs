namespace Pentagon.EntityFrameworkCore.Extensions {
    using System;
    using System.Linq;
    using System.Reflection;
    using Interfaces.Entities;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class ModelConfiguration : IModelConfiguration
    {
        /// <inheritdoc />
        protected abstract string DefaultCreateGuidDatabaseFunction { get; }

        public ModelBuilder SetupModel([NotNull] ModelBuilder builder, string providerName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            foreach (var type in builder.Model.GetEntityTypes())
            {
                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity)))
                    SetupId(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreateStampSupport)))
                    SetupUuid(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IConcurrencyStampSupport)))
                    SetupConcurrencyStamp(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreateTimeStampSupport)))
                    SetupCreatedAt(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICreatedUserEntitySupport)))
                    SetupCreatedUser(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IUpdateTimeStampSupport)))
                    SetupUpdatedAt(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IUpdatedUserEntitySupport)))
                    SetupUpdatedUser(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeletedUserEntitySupport)))
                    SetupDeletedUser(builder, type.ClrType);

                if (type.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeletedFlagSupport)))
                    SetupDeleteFlag(builder, type.ClrType);
            }

            SetupModelCore(builder);

            return builder;
        }

        public ModelBuilder SetupId<T>(ModelBuilder builder)
                where T : class, IEntity
            => SetupId(builder, typeof(T));

        public ModelBuilder SetupId(ModelBuilder builder, Type type)
        {
            var entity = builder.Entity(type);

            entity.HasKey(nameof(IEntity.Id));

            entity.Property(nameof(IEntity.Id))
                  .ValueGeneratedOnAdd()
                  .IsRequired();

            if (entity.Property(nameof(IEntity.Id))?.Metadata?.ClrType == typeof(Guid))
            {
                entity.Property(nameof(IEntity.Id))
                      .HasDefaultValueSql(sql: DefaultCreateGuidDatabaseFunction);
            }

            SetupIdCore(entity, type);

            return builder;
        }

        protected virtual void SetupIdCore(EntityTypeBuilder builder, Type type) { }

        protected virtual void SetupModelCore(ModelBuilder builder) { }

        public ModelBuilder SetupConcurrencyStamp<T>(ModelBuilder builder)
                where T : class, IConcurrencyStampSupport
            => SetupConcurrencyStamp(builder, typeof(T));

        public ModelBuilder SetupConcurrencyStamp(ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IConcurrencyStampSupport.ConcurrencyStamp))
                   .HasDefaultValueSql(sql: DefaultCreateGuidDatabaseFunction)
                   .IsRequired();

            return builder;
        }

        public ModelBuilder SetupCreatedAt<T>(ModelBuilder builder)
                where T : class, ICreateTimeStampSupport
            => SetupCreatedAt(builder, typeof(T));

        public ModelBuilder SetupCreatedAt(ModelBuilder builder, Type type)
        {
            builder.Entity(type).Property(nameof(ICreateTimeStampSupport.CreatedAt))
                   .HasDefaultValueSql(sql: "SYSDATETIMEOFFSET()")
                   .IsRequired();

            return builder;
        }

        public ModelBuilder SetupUpdatedAt<T>(ModelBuilder builder)
                where T : class, IUpdateTimeStampSupport
            => SetupUpdatedAt(builder, typeof(T));

        public ModelBuilder SetupUpdatedAt(ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IUpdateTimeStampSupport.UpdatedAt))
                   .IsRequired(false);

            return builder;
        }

        public ModelBuilder SetupCreatedUser<T>(ModelBuilder builder)
                where T : class, ICreatedUserEntitySupport
            => SetupCreatedUser(builder, typeof(T));

        public ModelBuilder SetupCreatedUser(ModelBuilder builder, Type type)
        {
            builder.Entity(type).Property(nameof(ICreatedUserEntitySupport.CreatedUser))
                   .HasMaxLength(256);

            return builder;
        }

        public ModelBuilder SetupUpdatedUser<T>(ModelBuilder builder)
                where T : class, IUpdatedUserEntitySupport
            => SetupUpdatedUser(builder, typeof(T));

        public ModelBuilder SetupUpdatedUser(ModelBuilder builder, Type type)
        {
            builder.Entity(type).Property(nameof(IUpdatedUserEntitySupport.UpdatedUser))
                   .HasMaxLength(256);

            return builder;
        }

        public ModelBuilder SetupDeletedUser<T>(ModelBuilder builder)
                where T : class, IDeletedUserEntitySupport
            => SetupDeletedUser(builder, typeof(T));

        public ModelBuilder SetupDeletedUser(ModelBuilder builder, Type type)
        {
            builder.Entity(type).Property(nameof(IDeletedUserEntitySupport.DeletedUser))
                   .HasMaxLength(256);

            return builder;
        }

        public ModelBuilder SetupUuid<T>(ModelBuilder builder)
                where T : class, ICreateStampSupport =>
                SetupUuid(builder, typeof(T));

        public ModelBuilder SetupUuid(ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(ICreateStampSupport.Uuid))
                   .IsRequired()
                   .HasDefaultValueSql(sql: DefaultCreateGuidDatabaseFunction)
                   .HasColumnName(name: "UUID");

            return builder;
        }

        public ModelBuilder SetupDeleteFlag<T>(ModelBuilder builder)
                where T : class, ICreateStampSupport => SetupDeleteFlag(builder, typeof(T));

        public ModelBuilder SetupDeleteFlag(ModelBuilder builder, Type type)
        {
            builder.Entity(type)
                   .Property(nameof(IDeletedFlagSupport.DeletedFlag))
                   .HasDefaultValue(0)
                   .IsRequired();

            return builder;
        }
    }
}