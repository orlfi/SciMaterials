using AutoMapper;
using SciMaterials.Contracts.API.DTO.Files;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.Contracts.API.Mappings;

public class FileProfile:Profile
{
	public FileProfile()
	{
        CreateMap<File, GetFileResponse>().ReverseMap();
        CreateMap<File, AddEditFileRequest>().ReverseMap();
        CreateMap<File, FileMetadata>().ReverseMap();
    }
}
