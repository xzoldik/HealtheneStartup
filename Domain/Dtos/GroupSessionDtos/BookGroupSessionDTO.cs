using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.GroupSessionDtos
{
    public class BookGroupSessionDto
    {
        [Required(ErrorMessage = "Therapist ID is required.")]
        public int TherapistID { get; set; }
        [Required(ErrorMessage = "Scheduled start time is required.")]
        public DateTime scheduledStartTime { get; set; }
        [Required(ErrorMessage = "Duration end time is required.")]
        public int duration { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Max Participants is required.")]
        [Range(2, 50)]
        public required int MaxParticipants { get; set; }
        public List<ParticipantDTO>? Participants { get; set; }
    }
}

