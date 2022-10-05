﻿using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Comment : BaseModel
{
    public Guid? ParentId { get; set; }
    public Guid OwnerId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public Guid? FileId { get; set; }
    public Guid? FileGroupId { get; set; }

    public User Owner { get; set; } = null!;
    public File? File { get; set; } = null!;
    public FileGroup? FileGroup { get; set; } = null!;
    public Comment()
    {
    }
}
