using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace SciMaterials.LinkSearch.WebAPI.Models
{
    [Table("LinkSearch")]
    public class LinkSearch
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text { get; set; }
        
        public LinkSearch()
        {
            if (Text != null)
                Text = Regex.Replace(Text,
                    @"((http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?)",
                    "\"$1\"");
        }
    }
}