namespace Pentagon.EntityFrameworkCore {
    using Microsoft.EntityFrameworkCore;

    public static class EntryStateExtensions
    {
        public static EntityStateType ToEntityStateType(this EntityState state)
        {
            switch (state)
            {
                case EntityState.Detached:
                case EntityState.Deleted:
                    return EntityStateType.Deleted;

                case EntityState.Modified:
                    return EntityStateType.Modified;

                case EntityState.Added:
                    return EntityStateType.Added;
                    
                default:
                    return EntityStateType.Unspecified;
            }
        }
    }
}