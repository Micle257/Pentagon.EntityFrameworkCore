namespace Pentagon.Data.EntityFramework.Abstractions.Entities {
    public interface IUserIdentitySupport<TUserId, TUser>
    {
        TUserId UserId { get; set; }

        TUser User { get; set; }
    }
}