// -----------------------------------------------------------------------
//  <copyright file="IDataUserIdentityWriter.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces
{
    using JetBrains.Annotations;

    /// <summary>
    /// Represents a services that can write current user's identity used in identity related fields in models.
    /// </summary>
    public interface IDataUserIdentityWriter
    {
        void SetIdentity([NotNull] object id, string userName);
    }
}