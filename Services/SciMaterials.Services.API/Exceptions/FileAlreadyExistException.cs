using SciMaterials.Contracts.Enums;

namespace SciMaterials.Services.API.Exceptions;

public class FileAlreadyExistException : ApiException
{
    public string FileName { get; }

    public FileAlreadyExistException(string fileName) : this(fileName, $"File {fileName} already exist") { }
    public FileAlreadyExistException(string fileName, string message) : base((int)ResultCodes.FileAlreadyExist, message)
       => FileName = fileName;
}
