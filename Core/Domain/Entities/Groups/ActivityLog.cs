using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Groups
{
    public class ActivityLog : BaseEntity<int>
    {
        public string ActionType { get; set; }
        public int? PointsEarned { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public virtual Group Group { get; set; }

        #endregion
    }
}
