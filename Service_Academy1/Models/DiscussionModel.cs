using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class PostModel
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key to the Program
        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; }

        [ForeignKey("Author")]
        public string? AuthorId { get; set; } // ID of the user who posted

        public virtual ProgramsModel? Program { get; set; }
        public virtual ApplicationUser? Author { get; set; }
        public virtual ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }

    public class CommentModel
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key to the post
        [ForeignKey("PostModel")]
        public int PostId { get; set; }

        [ForeignKey("Author")]
        public string? AuthorId { get; set; } // ID of the user who commented

        public virtual PostModel? Post { get; set; }
        public virtual ApplicationUser? Author { get; set; }
    }
}
