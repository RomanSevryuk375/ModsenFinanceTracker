namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IValidator<T>
{
    (bool IsValid, string Message) Validate(T entity);
}