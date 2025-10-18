using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities.Posts
{
    public class Post : BaseEntity<int>
    {
        #region ATTENTION!! Important Note About Creating Post
        //In the Service Layer(Recommended)
        //If you have a service handling post creation(like PostService.CreatePost()),
        //put it before saving:

        //public void CreatePost(Post post)
        //{
        //    if (string.IsNullOrWhiteSpace(post.Caption) && string.IsNullOrWhiteSpace(post.MediaURL))
        //        throw new InvalidOperationException("Post must have either text or media.");

        //    _context.Posts.Add(post);
        //    _context.SaveChanges();
        //} 
        #endregion
        public string? Caption { get; set; }

        [StringLength(255)]
        public string? MediaURL { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public AccessibilityType Accessibility { get; set; } = AccessibilityType.Public;

        // Navigation properties
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
        [Required]
        public int GroupId { get; set; }

        public virtual AIModeration AI_Moderation { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
       
    }
}
