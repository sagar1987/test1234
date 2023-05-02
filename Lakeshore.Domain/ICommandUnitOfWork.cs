
namespace Lakeshore.Domain;

public interface ICommandUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
