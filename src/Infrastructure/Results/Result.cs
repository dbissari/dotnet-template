using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Results;

public sealed class Result : Result<Result>
{
    private Result()
        : base(true, new Result(), Fault.None) { }

    public static Result<Result> Success() => Success<Result>(new());

    public static Result<T> Success<T>(T value) => new(true, value, Fault.None);

    public static Result<Result> Failure(Fault fault) => Failure<Result>(fault);

    public static Result<T> Failure<T>(Fault fault) => new(false, default, fault);
}

public class Result<T>
{
    public Result(bool isSuccess, T? value, Fault fault)
    {
        switch (isSuccess)
        {
            case true when fault != Fault.None:
                throw new InvalidOperationException("Success result cannot have a fault.");
            case false when fault == Fault.None:
                throw new InvalidOperationException("Failure result must have a fault.");
            case true when value is null:
                throw new InvalidOperationException("Success result cannot have a null value.");
            case false when value is not null:
                throw new InvalidOperationException("Failure result cannot have a value.");
            default:
                IsSuccess = isSuccess;
                Value = value;
                Fault = fault;
                break;
        }
    }

    public T? Value { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Fault))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Fault))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure => !IsSuccess;

    public Fault Fault { get; }

    public TMatch Match<TMatch>(Func<TMatch> onSuccess, Func<Fault, TMatch> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess() : onFailure(Fault);
    }
}
