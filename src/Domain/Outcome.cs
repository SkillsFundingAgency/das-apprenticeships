namespace SFA.DAS.Apprenticeships.Domain;

public class Outcome
{
    public bool IsSuccess { get; private set; }
    public object? Result { get; private set; }

    public Outcome(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public Outcome(bool isSuccess, object result)
    {
        IsSuccess = isSuccess;
        Result = result;
    }

    public T GetResult<T>()
    {
        if(Result is null)
        {
            throw new InvalidOperationException("Outcome.GetResult called on an Outcome object with a null Result.");
        }

        if (Result is T result)
        {
            return result;
        }
        throw new InvalidCastException($"Outcome.GetResult unable to cast object of type {Result.GetType()} to type {typeof(T)}.");
    }

    public static Outcome Success()
    {
        return new Outcome(true);
    }

    public static Outcome Success(object result)
    {
        return new Outcome(true, result);
    }

    public static Outcome Fail()
    {
        return new Outcome(false);
    }

    public static Outcome Fail(object result)
    {
        return new Outcome(false, result);
    }
}