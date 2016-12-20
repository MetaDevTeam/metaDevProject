using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Recipe
    {
        private ICollection<RecipeTag> recipeTags;
        private ICollection<Ingredient> ingredients;

        public Recipe()
        {
            this.recipeTags = new HashSet<RecipeTag>();
            this.ingredients = new HashSet<Ingredient>();
        }

        public Recipe(string authorId, string title, string context, int recipeCategoryId)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Content = context;
            this.RecipeCategoryId = recipeCategoryId;
            this.recipeTags = new HashSet<RecipeTag>();
            this.ingredients = new HashSet<Ingredient>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        [ForeignKey("RecipeCategory")]
        public int RecipeCategoryId { get; set; }

        public virtual RecipeCategory RecipeCategory { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }

        public virtual ICollection<RecipeTag> RecipeTags
        {
            get { return this.recipeTags; }
            set { this.recipeTags = value; }
        }

        public virtual ICollection<Ingredient> Ingredients
        {
            get { return this.ingredients; }
            set { this.ingredients = value; }
        }
    }
}