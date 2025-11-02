using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApplication.Models
{
    public class Comment
    {
        [Key] //Validation attribute to specify primary key
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName is required")] //Validation attribute to specify required field
        [MaxLength(100, ErrorMessage = "UserName cannot exceed 100 characters")] //Validation attribute to specify maximum length
        public string? UserName { get; set; }

        [Required]
        public string? CommentContent { get; set; }

        [DataType(DataType.Date)] //Validation attribute to specify data type
        public DateTime CommentDate { get; set; } = DateTime.Now;

        [ForeignKey("Post")] //specifies foreign key relationship
        public int PostId { get; set; } //foreign key property

        public Post? Post { get; set; } //Navigation property for related post
    }
}
