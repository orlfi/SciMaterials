using SciMaterials.Contracts.Exceptions;

namespace SciMaterials.Contracts.Result;

/// <summary>Возвращает результат операции</summary>
public class Result : IResult
{
    /// <summary> Код результата. </summary>
    public int Code { get; set; }

    /// <summary> Сообщение о результате выполнение операции. </summary>
    public ICollection<string> Messages { get; set; } = new List<string>();

    /// <summary> Успешность выполнения операции (true - успешно | false - ошибка) </summary>
    public bool Succeeded { get; set; }

    /// <summary> Возращает успешный результат выполнения операции. </summary>
    /// <returns> Результат операции. </returns>
    public static Result Success() => new() { Succeeded = true };

    /// <summary> Возращает успешный результат выполнения операции. </summary>
    /// <param name="message"> Текствое сообщение о результате выполнения операции. </param>
    /// <returns> Результат операции. </returns>
    public static Result Success(string message) => new() { Succeeded = true, Messages = new List<string> { message } };

    /// <summary> Возращает успешный результат выполнения операции. Асинхронный. </summary>
    /// <returns> Результат операции. </returns>
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());
    
    /// <summary> Возращает успешный результат выполнения операции. Асинхронный. </summary>
    /// <param name="message"> Текствое сообщение о результате выполнения операции. </param>
    /// <returns> Результат операции. </returns>
    public static Task<Result> SuccessAsync(string message) => Task.FromResult(Success(message));

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Error(int code) => new() { Succeeded = false, Code = code };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="message"> Дополнительно сообщение об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Error(int code, string message) => new() { Succeeded = false, Code = code, Messages = new List<string> { message } };

    /// <summary> Возращает результат с ошибкой выполнения операции. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="messages"> Дополнительные сообщения об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Result Error(int code, ICollection<string> messages) => new Result() { Succeeded = false, Code = code, Messages = messages };

    /// <summary> Возращает результат с ошибкой выполнения операции. Асинхронный. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Task<Result> ErrorAsync(int code) => Task.FromResult(Error(code));

    /// <summary> Возращает результат с ошибкой выполнения операции. Асинхронный. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="message"> Дополнительно сообщение об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Task<Result> ErrorAsync(int code, string message) => Task.FromResult(Error(code, message));

    /// <summary> Возращает результат с ошибкой выполнения операции. Асинхронный. </summary>
    /// <param name="code"> Код ошибки. </param>
    /// <param name="messages"> Дополнительные  сообщения об ошибке. </param>
    /// <returns> Результат с ошибкой операции. </returns>
    public static Task<Result> ErrorAsync(int code, ICollection<string> messages) => Task.FromResult(Error(code, messages));
}

public class Result<TData> : Result, IResult<TData>
{
    public TData? Data { get; set; }

    public static new Result<TData> Success() => new() { Succeeded = true };

    public static new Result<TData> Success(string message) => new() { Succeeded = true, Messages = new List<string> { message } };

    public static Result<TData> Success(TData data) => new() { Data = data, Succeeded = true };

    public static Result<TData> Success(TData data, string message) => new() { Data = data, Succeeded = true, Messages = new List<string> { message } };

    public static new Task<Result<TData>> SuccessAsync() => Task.FromResult(Success());

    public static new Task<Result<TData>> SuccessAsync(string message) => Task.FromResult(Success(message));

    public static Task<Result<TData>> SuccessAsync(TData data) => Task.FromResult(Success(data));

    public static Task<Result<TData>> SuccessAsync(TData data, string message) => Task.FromResult(Success(data, message));

    public static new Result<TData> Error(int code) => new() { Succeeded = false, Code = code };

    public static new Result<TData> Error(int code, string message) => new() { Succeeded = false, Code = code, Messages = new List<string> { message } };

    public static new Result<TData> Error(int code, ICollection<string> messages) => new Result<TData>() { Succeeded = false, Code = code, Messages = messages };

    public static new Task<Result<TData>> ErrorAsync(int code) => Task.FromResult(Error(code));

    public static new Task<Result<TData>> ErrorAsync(int code, string message) => Task.FromResult(Error(code, message));

    public static new Task<Result<TData>> ErrorAsync(int code, ICollection<string> messages) => Task.FromResult(Error(code, messages));

    public static implicit operator Result<TData>(TData data) => new() { Succeeded = true, Data = data };

    public static implicit operator Result<TData>(ApiException exception) => new() { Succeeded = false, Code = exception.Code, Messages = new List<string> { exception.Message } };
}
