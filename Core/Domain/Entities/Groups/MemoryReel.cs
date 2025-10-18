using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Groups
{
    public class MemoryReel : BaseEntity<int>
    {
        public string VideoURL { get; set; }
        public int? Year { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public virtual Group Group { get; set; }

        #endregion
    }
}
