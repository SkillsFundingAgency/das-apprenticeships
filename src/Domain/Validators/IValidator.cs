namespace SFA.DAS.Apprenticeships.Domain.Validators;

public interface IValidator<T>
{
    bool IsValid(T entity, out string validationMessage, params object?[] args);
}
