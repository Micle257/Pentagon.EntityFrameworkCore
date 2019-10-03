// -----------------------------------------------------------------------
//  <copyright file="DataUserProvider.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore
{
    using Interfaces;
    using Microsoft.Extensions.Logging;

    public class DataUserProvider : IDataUserProvider
    {
        readonly ILogger<DataUserProvider> _logger;
        object _userId;
        string _userName;

        public DataUserProvider(ILogger<DataUserProvider> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public object UserId
        {
            get => _userId;
            set
            {
                _logger.LogDebug($"Setting user id to: {value} (previous value: {_userId})");
                _userId = value;
            }
        }

        /// <inheritdoc />
        public string UserName
        {
            get => _userName;
            set
            {
                _logger.LogDebug($"Setting user name to: {value} (previous value: {_userName})");
                _userName = value;
            }
        }
    }
}