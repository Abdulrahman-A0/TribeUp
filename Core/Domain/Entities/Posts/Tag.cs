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
    public class Tag : BaseEntity<int>
    {
        // Navigation properties
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        [Required]
        public int PostId { get; set; }

        /// ////////

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
