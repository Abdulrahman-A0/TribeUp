using Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Stories
{
    public class StoryView : BaseEntity<int>
    {
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        // Navigation properties
        [ForeignKey("StoryId")]
        public virtual Story Story { get; set; }
        public int StoryId { get; set; }

        ///////

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
