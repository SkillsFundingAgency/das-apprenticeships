namespace SFA.DAS.Apprenticeships.Command;

public interface IValidator<T>
{
    bool IsValid(T entity, out string validationMessage, params object?[] args);
}
