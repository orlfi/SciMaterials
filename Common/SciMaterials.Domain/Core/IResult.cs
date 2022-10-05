namespace SciMaterials.Domain.Core;

public interface IResult
{
    bool Succeeded { get; set; }
    ICollection<string> Messages { get; set; }
}

public interface IResult<out TData>: IResult
{
    TData Data { get; }
}