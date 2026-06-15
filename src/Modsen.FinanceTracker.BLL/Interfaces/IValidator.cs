using Modsen.FinanceTracker.Domain;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IValidator<T>
{
    public Result<T> Validate(T entity);
}