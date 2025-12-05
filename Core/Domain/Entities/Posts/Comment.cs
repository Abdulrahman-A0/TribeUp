using Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Posts
{
    public class Comment : BaseEntity<int>
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // Navigation properties
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        [Required]
        public int PostId { get; set; }

        /// //////////

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
