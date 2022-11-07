namespace SciMaterials.Contracts.Result;

/// <summary>Возвращает результат операции</summary>
public class PageResult<TData> : Result
{
    private const int PageSizeDefault = 10;
    public List<TData>? Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = PageSizeDefault;
    public int TotalCount { get; set; }
    public int TotalPages => TotalCount / PageSize;

    public static new PageResult<TData> Success(List<TData> data, int pageNumber, int pageSize, int totalCount) => new()
    {
        //Succeeded = true,
        Data = data,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalCount = totalCount
    };

    public static Task<PageResult<TData>> SuccessAsync(List<TData> data, int pageNumber = 1, int pageSize = PageSizeDefault, int totalCount = 0) => Task.FromResult(Success(data, pageNumber, pageSize, totalCount));

    public static new Result<TData> Error(int code) => new() { Code = code };

    public static new Result<TData> Error(int code, string message) => new() { Code = code, Message = message };

    //public static new Result<TData> Error(int code, ICollection<string> messages) => new Result<TData>() { Succeeded = false, Code = code, Messages = messages };

    public static new Task<Result<TData>> ErrorAsync(int code) => Task.FromResult(Error(code));

    public static new Task<Result<TData>> ErrorAsync(int code, string message) => Task.FromResult(Error(code, message));

    //public static new Task<Result<TData>> ErrorAsync(int code, ICollection<string> messages) => Task.FromResult(Error(code, messages));
}