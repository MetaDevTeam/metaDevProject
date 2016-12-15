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
        private ICollection<Tag> tags;

        public Recipe()
        {
            this.tags = new HashSet<Tag>();
        }

        public Recipe(string authorId, string title, string context, int recipeCategoryId)
        {
            this.AuthorId = authorId;
            this.Title = title;
            this.Content = context;
            this.RecipeCategoryId = recipeCategoryId;
            this.tags = new HashSet<Tag>();
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

        public virtual ICollection<Tag> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }
    }
}