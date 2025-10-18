using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Media
{
    public class Media : BaseEntity<int>
    {
        public string MediaURL { get; set; }
        public string Type { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public virtual Album Album { get; set; }
        #endregion
    }
}
