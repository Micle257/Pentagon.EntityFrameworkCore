namespace Pentagon.EntityFrameworkCore.Extensions {
    using System;
    using Interfaces.Entities;
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;

    public interface IModelConfiguration
    {
        ModelBuilder SetupModel([NotNull] ModelBuilder builder, string providerName);

        ModelBuilder SetupId<T>(ModelBuilder builder)
                where T : class, IEntity;

        ModelBuilder SetupId(ModelBuilder builder, Type type);

        ModelBuilder SetupConcurrencyStamp<T>(ModelBuilder builder)
                where T : class, IConcurrencyStampSupport;

        ModelBuilder SetupConcurrencyStamp(ModelBuilder builder, Type type);

        ModelBuilder SetupCreatedAt<T>(ModelBuilder builder)
                where T : class, ICreateTimeStampSupport;

        ModelBuilder SetupCreatedAt(ModelBuilder builder, Type type);

        ModelBuilder SetupUpdatedAt<T>(ModelBuilder builder)
                where T : class, IUpdateTimeStampSupport;

        ModelBuilder SetupUpdatedAt(ModelBuilder builder, Type type);

        ModelBuilder SetupCreatedUser<T>(ModelBuilder builder)
                where T : class, ICreatedUserEntitySupport;

        ModelBuilder SetupCreatedUser(ModelBuilder builder, Type type);

        ModelBuilder SetupUpdatedUser<T>(ModelBuilder builder)
                where T : class, IUpdatedUserEntitySupport;

        ModelBuilder SetupUpdatedUser(ModelBuilder builder, Type type);

        ModelBuilder SetupDeletedUser<T>(ModelBuilder builder)
                where T : class, IDeletedUserEntitySupport;

        ModelBuilder SetupDeletedUser(ModelBuilder builder, Type type);

        ModelBuilder SetupUuid<T>(ModelBuilder builder)
                where T : class, ICreateStampSupport;

        ModelBuilder SetupUuid(ModelBuilder builder, Type type);

        ModelBuilder SetupDeleteFlag<T>(ModelBuilder builder)
                where T : class, ICreateStampSupport;

        ModelBuilder SetupDeleteFlag(ModelBuilder builder, Type type);
    }
}