
using System.Xml.Linq;

namespace SciMaterials.Domain.Core;

public class Result : IResult
{
    public ICollection<string> Messages { get; set; } = new List<string>();

    public bool Succeeded { get; set; }

    public static IResult Success() => new Result() { Succeeded = true };
    
    public static IResult Success(string message) => new Result() { Succeeded = true, Messages = new List<string> { message } };

    public static Task<IResult> SuccessAsync() => Task.FromResult(Success());

    public static Task<IResult> SuccessAsync(string message) => Task.FromResult(Success(message));

    public static IResult Error() => new Result() { Succeeded = false };

    public static IResult Error(string message) => new Result() { Succeeded = false, Messages = new List<string> { message } };

    public static Task<IResult> ErrorAsync() => Task.FromResult(Error());

    public static Task<IResult> ErrorAsync(string message) => Task.FromResult(Error(message));
}

public class Result<TData> : Result, IResult<TData>
{
    public TData Data { get; set; } = default!;

    public static new IResult<TData> Success() => new Result<TData>() { Succeeded = true };

    public static new IResult<TData> Success(string message) => new Result<TData>() { Succeeded = true, Messages = new List<string> { message } };

    public static IResult<TData> Success(TData data) => new Result<TData>() { Data = data, Succeeded = true };

    public static IResult<TData> Success(TData data, string message) => new Result<TData>() { Data = data, Succeeded = true, Messages = new List<string> { message } };

    public static new Task<IResult<TData>> SuccessAsync() => Task.FromResult(Success());

    public static new Task<IResult<TData>> SuccessAsync(string message) => Task.FromResult(Success(message));
    
    public static Task<IResult<TData>> SuccessAsync(TData data) => Task.FromResult(Success(data));
    
    public static Task<IResult<TData>> SuccessAsync(TData data, string message) => Task.FromResult(Success(data, message));

    public static new IResult<TData> Error() => new Result<TData>() { Succeeded = false };

    public static new IResult<TData> Error(string message) => new Result<TData>() { Succeeded = false, Messages = new List<string> { message } };

    public static new Task<IResult<TData>> ErrorAsync() => Task.FromResult(Error());

    public static new Task<IResult<TData>> ErrorAsync(string message) => Task.FromResult(Error(message));

    public static implicit operator Result<TData>(TData data) => new() { Succeeded = true, Data = data };
    
    public static implicit operator Result<TData>(Exception exception) => new() { Succeeded = false, Messages = new List<string> { exception.Message } };

}
