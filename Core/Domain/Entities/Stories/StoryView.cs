using Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Stories
{
    public class StoryView
    {
        public DateTime ViewedAt { get; set; } = DateTime.Now;
        // Navigation properties
        [ForeignKey("StoryId")]
        public virtual Story Story { get; set; }
        public int StoryId { get; set; }
        
        ///////

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }
}
