using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.EventModule
{
    public class EventResponseDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime EventDate { get; set; }

        public string Location { get; set; }

        public EventStatus Status { get; set; }

        public int GoingCount { get; set; }

        public int InterestedCount { get; set; }

        public int NotGoingCount { get; set; }

        public string CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
