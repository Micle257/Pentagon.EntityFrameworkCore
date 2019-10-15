namespace Pentagon.EntityFrameworkCore.TestApp {
    using System;
    using Interfaces.Entities;

    class User : Entity, ICreateStampSupport,
                 IConcurrencyStampSupport,
                 IUpdatedUserEntitySupport, IUpdateTimeStampSupport, IUpdateTimeStampIdentitySupport<int?>,
                 ICreatedUserEntitySupport, ICreateTimeStampSupport, ICreateTimeStampIdentitySupport<int?>
    {
        int _updatedUserId;

        /// <inheritdoc />
        public Guid Uuid { get; set; }

        public string Name { get; set; }

        public int? a1 { get; set; }

        public double? a2 { get; set; }

        public char a3 { get; set; }

        /// <inheritdoc />
        public Guid ConcurrencyStamp { get; set; }

        /// <inheritdoc />
        public string UpdatedUser { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <inheritdoc />
        object IUpdateTimeStampIdentitySupport.UpdatedUserId
        {
            get => UpdatedUserId;
            set => UpdatedUserId = (int?) value;
        }

        /// <inheritdoc />
        public int? UpdatedUserId { get; set; }

        /// <inheritdoc />
        public string CreatedUser { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; set; }

        /// <inheritdoc />
        object ICreateTimeStampIdentitySupport.CreatedUserId
        {
            get => CreatedUserId;
            set => CreatedUserId = (int?) value;
        }

        /// <inheritdoc />
        public int? CreatedUserId { get; set; }
    }
}