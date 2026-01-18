using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BazaR.Backend.Domain.Common;

public class Result
{
    private readonly List<Error> _errors;

    protected Result(bool isSuccess, IEnumerable<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        _errors = errors?.ToList() ?? new List<Error>();
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error => _errors.Count > 0 ? _errors[0] : Error.None;
    public IReadOnlyList<Error> Errors => _errors;

    public static Result Success() => new(true);

    public static Result Failure(Error error) => new(false, new[] { error });

    public static Result Failure(IEnumerable<Error> errors)
        => new(false, errors ?? throw new ArgumentNullException(nameof(errors)));

    public static Result Validation(params Error[] errors)
        => new(false, errors.Select(e => e with { Type = ErrorType.Validation }));

    public static Result Combine(params Result[] results)
    {
        var errors = results
            .Where(r => r is not null && r.IsFailure)
            .SelectMany(r => r.Errors)
            .ToList();

        return errors.Count == 0 ? Success() : Failure(errors);
    }
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, IEnumerable<Error>? errors = null)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value);

    public static new Result<T> Failure(Error error) => new(false, default, new[] { error });

    public static Result<T> Failure(IEnumerable<Error> errors) => new(false, default, errors);

    public static Result<T> Validation(params Error[] errors)
        => new(false, default, errors.Select(e => e with { Type = ErrorType.Validation }));
}
