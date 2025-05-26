using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.FeedbackDtos
{
    public class FeedbackDto
    {
        [Required(ErrorMessage = "Feedback ID is required.")]
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Therapist ID is required.")]
        public int TherapistId { get; set; }
        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Review text is required.")]
        public string ReviewText { get; set; }
        public int? SessionId { get; set; } // Nullable int for individual sessions
        public int? GroupSessionId { get; set; } // Nullable int for group sessions
        [Required(ErrorMessage = "Session type is required.")]
        public required string SessionType { get; set; }
    }
}
