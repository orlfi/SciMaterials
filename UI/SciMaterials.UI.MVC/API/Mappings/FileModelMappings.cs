//using SciMaterials.UI.MVC.API.DTO.Files;
//using SciMaterials.UI.MVC.API.Models;

//namespace SciMaterials.UI.MVC.API.Mappings;

//public static class FileModelMappings
//{
//    public static FileUploadResponse? ToViewModel(this FileModel? model)
//        => model is null
//            ? null
//            : new FileUploadResponse()
//            {
//                Hash = model.Hash!,
//                FileName = model.FileName,
//                ContentType = model.ContentType,
//                Size = model.Size
//            };
//}