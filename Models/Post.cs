using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApplication.Models
{
    public class Post
    {
        [Key] //Validation attribute to specify primary key
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")] //Validation attribute to specify required field
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")] //Validation attribute to specify maximum length
        public string? Title { get; set; }

        [Required(ErrorMessage = "Title is required")] //Validation attribute to specify required field
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")] //Validation attribute to specify maximum length
        public string? Author { get; set; }

        [Required(ErrorMessage = "Content is required")] //Validation attribute to specify required field
        public string? Content { get; set; }

        [ValidateNever]
        public string? FeatureImagePath { get; set; }

        [DataType(DataType.Date)] //Validation attribute to specify data type
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [ForeignKey("Category")] //specifies foreign key relationship]
        public int CategoryId { get; set; } //foreign key property

        [ValidateNever]
        public Category? Category { get; set; } //navigation property for that foreign key relationship

        public ICollection<Comment>? Comments { get; set; } //Navigation property for related comments

    }
}
