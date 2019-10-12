// -----------------------------------------------------------------------
//  <copyright file="DataUserProvider.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using System;
    using Interfaces;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    public class DataUserProvider : IDataUserProvider, IDataUserIdentityWriter
    {
        readonly ILogger<DataUserProvider> _logger;
        object _userId;
        string _userName;

        public DataUserProvider(ILogger<DataUserProvider> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public object UserId => _userId;

        /// <inheritdoc />
        public string UserName => _userName;

        /// <inheritdoc />
        public void SetIdentity( object id, string userName)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            _logger.LogDebug("Setting user identity: ID {Id}, Name {Name} (previous value: ID {PrevId}, Name {PrevName})", id, userName, _userId, _userName);

            _userId = id;
            _userName = userName;
        }
    }
}