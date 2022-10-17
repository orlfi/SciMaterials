using AutoMapper;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.Contracts.API.Mappings;

public class FileProfile : Profile
{
    public FileProfile()
    {
        CreateMap<File, GetFileResponse>().ReverseMap();
        CreateMap<File, EditFileRequest>().ReverseMap();
        CreateMap<UploadFileRequest, FileMetadata>().ReverseMap();
        CreateMap<FileMetadata, File>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<EditFileRequest, File>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ReverseMap();
    }
}
