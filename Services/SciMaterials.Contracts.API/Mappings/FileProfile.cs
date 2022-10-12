using AutoMapper;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.UI.MVC.API.Mappings;

public class FileProfile : Profile
{
    public FileProfile()
    {
        CreateMap<File, GetFileResponse>().ReverseMap();
        CreateMap<File, EditFileRequest>().ReverseMap();
        CreateMap<UploadFileRequest, FileMetadata>().ReverseMap();
    }
}
