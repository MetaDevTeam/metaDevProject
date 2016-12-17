using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class RecipeCategory
    {
        private ICollection<Recipe> recipes;

        public RecipeCategory()
        {
            this.recipes = new HashSet<Recipe>();
        }

        [Key]
        public int id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Recipe> Recipes
        {
            get { return this.recipes; }
            set { this.recipes = value; }
        }
    }
}