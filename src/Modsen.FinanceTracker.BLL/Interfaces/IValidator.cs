using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IValidator<T>
{
    public Result<T> Validate(T entity);
}