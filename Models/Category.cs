using System.ComponentModel.DataAnnotations;

namespace BlogWebApplication.Models
{
    public class Category
    {
        [Key] //Validation attribute to specify primary key
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")] //Validation attribute to specify required field
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")] //Validation attribute to specify maximum length]
        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Post>? Posts { get; set; } //Navigation property for related posts
    }
}
