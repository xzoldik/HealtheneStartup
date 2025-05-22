using Domain.Dtos.GroupSessionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GroupSession
    {
        public int GroupSessionID { get; set; }
        public int TherapistID { get; set; }
        public string TherapistFirstName { get; set; } = string.Empty;
        public string TherapistLastName { get; set; } = string.Empty;
        public DateTime ScheduledStartTime { get; set; }
        public int Duration { get; set; }
        public string SessionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? MaxParticipants { get; set; }
        public List<ParticipantDTO>? Participants { get; set; } = new List<ParticipantDTO>();
    }
}
