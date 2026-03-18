using System.ComponentModel.DataAnnotations.Schema;

namespace BlogCore.DAL.Models;

using System.ComponentModel.DataAnnotations;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [ForeignKey("Post")]
    public int PostId { get; set; }

    public Post Post { get; set; } = null!;
}
