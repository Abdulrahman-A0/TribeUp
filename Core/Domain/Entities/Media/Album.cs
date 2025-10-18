using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Media
{
    public class Album : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public virtual Group Group { get; set; }
        public virtual ICollection<Media> Media { get; set; }

        #endregion
    }
}
