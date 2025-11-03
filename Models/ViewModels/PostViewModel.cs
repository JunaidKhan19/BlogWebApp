using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogWebApplication.Models.ViewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; } // For dropdown list of categories

        public IFormFile FeatureImage { get; set; } // For uploading a feature image over HTTP request
    }
}
