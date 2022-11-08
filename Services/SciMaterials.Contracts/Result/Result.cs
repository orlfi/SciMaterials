using System.Xml.Linq;

using SciMaterials.Contracts.Exceptions;

namespace SciMaterials.Contracts.Result;

/// <summary>Возвращает результат операции</summary>
public class Result : IApiResult
{
    /// <summary> Код результата. </summary>
    public int Code { get; init; } = 0;

    public string Message { get; init; } = string.Empty;

    /// <summary> Успешность выполнения операции (true - успешно | false - ошибка) </summary>
    public bool Succeeded => Code == 0 ;

    /// <summary> Возращает успешный результат выполнения операции. </summary>
    /// <returns> Результат операции. </returns>
    public static Result Success() => new();

    /// <summary> Возращает успешный результат выполнения операции. </summary>
    /// <param name="message"> Текствое сообщение о результате выполнения операции. </param>
    /// <returns> Результат операции. </returns>
    public static Result Success(string message) => new() { Message = message };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Error(int code) => new() { Code = code };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="message"> Дополнительно сообщение об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Error(int code, string message) => new() { Code = code, Message =  message  };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="error"> Объект ошибки. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Error(Error error) => Error(error.Code, error.Message);

    /// <summary> Возращает результат с ошибкой выполнения операции. Асинхронный. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="messages"> Дополнительные  сообщения об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    //public static Task<Result> ErrorAsync(int code, ICollection<string> messages) => Task.FromResult(Error(code, messages));
    public Task<Result> ToTask() => Task.FromResult(this);

    public static Task<Result> ErrorAsync(Error error) => Task.FromResult(Error(error));
    
    public static implicit operator Result(Error error) => Error(error);

    public static implicit operator Result((int t1, string t2) t) => Error(t.t1, t.t2);
}

public class Result<TData> : Result, IApiResult<TData>
{
    public TData? Data { get; init; }

    public static new Result<TData> Success() => new();

    //public static new Result<TData> Success(string message) => new() { Messages = new List<string> { message } };
    public static new Result<TData> Success(string message) => new() { Message = message };

    public static Result<TData> Success(TData data) => new() { Data = data };

    //public static Result<TData> Success(TData data, string message) => new() { Data = data, Messages = new List<string> { message } };
    public static Result<TData> Success(TData data, string message) => new() { Data = data, Message = message };

    public static new Task<Result<TData>> SuccessAsync(string message) => Task.FromResult(Success(message));

    public static Task<Result<TData>> SuccessAsync(TData data) => Task.FromResult(Success(data));

    public static Task<Result<TData>> SuccessAsync(TData data, string message) => Task.FromResult(Success(data, message));

    public static new Result<TData> Error(Error error) => new() { Code = error.Code, Message = error.Message };

    public static new Result<TData> Error(int code) => new() { Code = code };

    public static new Result<TData> Error(int code, string message) => new() { Code = code, Message = message };

    public static new Task<Result<TData>> ErrorAsync(int code) => Task.FromResult(Error(code));

    public static new Task<Result<TData>> ErrorAsync(int code, string message) => Task.FromResult(Error(code, message));

    public new Task<Result<TData>> ToTask() => Task.FromResult(this);

    public static implicit operator Result<TData>(TData data) => Success(data);

    public static implicit operator Result<TData>(ApiException exception) => Error(exception.Code, exception.Message);

    public static implicit operator Result<TData>(Error error) => Error(error);

    public static implicit operator Result<TData>((int ErrorCode, string ErrorMessage) t) => Error(t.ErrorCode, t.ErrorMessage);
}
