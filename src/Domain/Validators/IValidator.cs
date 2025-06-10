namespace SFA.DAS.Learning.Domain.Validators;

public interface IValidator<T>
{
    bool IsValid(T entity, out string validationMessage, params object?[] args);
}
