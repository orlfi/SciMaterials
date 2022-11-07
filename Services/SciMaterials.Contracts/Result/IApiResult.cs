namespace SciMaterials.Contracts.Result;

public interface IApiResult
{
    int Code { get; init; }
    bool Succeeded { get;  }
    string Message { get; init; }
    //ICollection<string> Messages { get; set; }
}

public interface IApiResult<out TData> : IApiResult
{
    TData Data { get; }
}