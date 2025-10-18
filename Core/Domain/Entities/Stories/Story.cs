using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities.Stories
{
    public class Story : BaseEntity<int>
    {
        #region ATTENTION!! Important Note About Creating Story
        //In the Service Layer(Recommended)
        //If you have a service handling story creation(like StoryService.CreateStory()),
        //put it before saving:

        //public void CreateStory(Story Story)
        //{
        //    if (string.IsNullOrWhiteSpace(Story.Caption) && string.IsNullOrWhiteSpace(Story.MediaURL))
        //        throw new InvalidOperationException("Story must have either text or media.");

        //    _context.Story.Add(Story);
        //    _context.SaveChanges();
        //} 
        #endregion
        public string? Caption { get; set; }
        public string? MediaURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ExpiresAt { get; set; } = DateTime.Now.AddDays(1);
        public int ViewsCount { get; set; } = 0;
        public AccessibilityType Accessibility { get; set; } 

        // Navigation properties
        [ForeignKey("PostId")]
        public virtual Group Group { get; set; }
        public int GroupId { get; set; }
        public virtual ICollection<StoryView> StoryViews { get; set; } = new List<StoryView>();
    }
}
