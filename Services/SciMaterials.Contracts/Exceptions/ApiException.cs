namespace SciMaterials.Contracts.Exceptions;

public class ApiException : Exception
{
    public int Code { get; set; }
    public ApiException(int code) : this(code, $"Exception with error code {code}") { }
    public ApiException(int code, string message) : base(message) => Code = code;
}