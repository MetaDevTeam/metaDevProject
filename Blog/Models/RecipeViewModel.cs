using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class RecipeViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        [Required]
        [Display(Name = "Recipe Category")]
        public int RecipeCategoryId { get; set; }

        public string RecipeTags { get; set; }

        public ICollection<Recipe> Ricepes { get; set; }

        public List<RecipeCategory> RecipeCategories { get; set; }

        public string Ingredients { get; set; }
    }
}