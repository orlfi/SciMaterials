using SciMaterials.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal class CommentHelper
{
    public static IEnumerable<Comment> GetMany()
    {
        yield return GetOne();
    }

    public static Comment GetOne()
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            ParentId = Guid.NewGuid(),
            //AuthorId = Guid.NewGuid(),
            Text = "Some text",
            CreatedAt = DateTime.UtcNow,

            //FileId = Guid.NewGuid(),
            //FileGroupId = Guid.NewGuid(),

            Author = new Author
            {
                Id = Guid.NewGuid(),
            },

            File =
            new File
            {
                Id = Guid.NewGuid(),
            },
            FileGroup = new FileGroup
            {
                Id = Guid.NewGuid(),
            },
        };
    }
}