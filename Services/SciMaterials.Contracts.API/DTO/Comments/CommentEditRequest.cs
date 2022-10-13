namespace SciMaterials.Contracts.API.DTO.Comments;

public class CommentEditRequest
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? FileId { get; set; }
    public Guid? FileGroupId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ParentId { get; set; }
}