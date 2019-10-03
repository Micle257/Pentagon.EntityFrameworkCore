// -----------------------------------------------------------------------
//  <copyright file="OrderConstants.cs">
//   Copyright (c) Michal Pokorný. All Rights Reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Pentagon.EntityFrameworkCore.Interfaces.Entities
{
    public static class OrderConstants
    {
        const int Init = 100;

        public const int Uuid = Init + 0;
        public const int ConcurrencyStamp = Init +1;
        public const int CreatedAt = Init +2;
        public const int CreatedUserId = Init +3;
        public const int CreatedUser = Init +4;
        public const int UpdatedAt = Init +5;
        public const int UpdatedUserId = Init +6;
        public const int UpdatedUser = Init +7;
        public const int DeletedAt = Init +8;
        public const int DeletedUserId = Init +9;
        public const int DeletedUser = Init +10;
        public const int DeletedFlag = Init +11;
    }
}