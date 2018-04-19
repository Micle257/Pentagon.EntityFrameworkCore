namespace Pentagon.EntityFrameworkCore.Synchronization {
    using Abstractions.Entities;

    public struct RepositoryAction<T>
            where T : class, IEntity
    {
        public RepositoryAction(RepositoryType repositoryType, T entity, TableActionType action)
        {
            RepositoryType = repositoryType;
            Entity = entity;
            Action = action;
        }

        public RepositoryType RepositoryType { get; }

        /// <summary> Gets the entity. </summary>
        /// <value> The <see cref="T" />. </value>
        public T Entity { get; }

        /// <summary> Gets the action. </summary>
        /// <value> The <see cref="TableActionType" />. </value>
        public TableActionType Action { get; }
    }
}