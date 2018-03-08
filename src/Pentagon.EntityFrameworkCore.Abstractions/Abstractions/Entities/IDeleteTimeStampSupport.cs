namespace Pentagon.Data.EntityFramework.Abstractions.Entities {
    using System;

    /// <summary>
    /// Represents an entity, that supports delete time stamp.
    /// </summary>
    public interface IDeleteTimeStampSupport
    {
        /// <summary> Gets or sets the deleted time. </summary>
        /// <value> The <see cref="DateTimeOffset" />. </value>
        DateTimeOffset? DeletedAt { get; set; }
    }

    public interface IDeleteTimeStampIdentitySupport<TUserId> : IDeleteTimeStampIdentitySupport
        where TUserId : struct
    {
        /// <summary> Gets or sets the user that deleted this entity. </summary>
        /// <value> The nullable <see cref="TUserId" />. </value>
        new TUserId? DeletedBy { get; set; }
    }

    public interface IDeleteTimeStampIdentitySupport
    {
        /// <summary> Gets or sets the user that deleted this entity. </summary>
        /// <value> The nullable <see cref="TUserId" />. </value>
       object DeletedBy { get; set; }
    }
}