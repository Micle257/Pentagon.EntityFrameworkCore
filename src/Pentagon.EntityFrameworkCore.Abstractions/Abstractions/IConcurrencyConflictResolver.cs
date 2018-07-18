namespace Pentagon.EntityFrameworkCore.Abstractions
{
    using System.Threading.Tasks;

    public interface IConcurrencyConflictResolver<TContext>
    {
        Task<ConcurrencyConflictResolveResult> ResolveAsync(IApplicationContext appContext);
    }
}