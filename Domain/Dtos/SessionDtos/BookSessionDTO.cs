using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.SessionDtos
{
    public class BookSessionDTO
    {
        [Required(ErrorMessage = "Session ID is required.")]
        public int therapistId { get; set; }
        [Required(ErrorMessage = "Patient ID is required.")]
        public int patientId { get; set; }
        [Required(ErrorMessage = "Scheduled start time is required.")]
        public DateTime scheduledStartTime { get; set; }
        [Required(ErrorMessage = "duration end time is required.")]
        public int duration { get; set; }
        [Required(ErrorMessage = "Session type is required.")]
        [RegularExpression(@"^(فردي|جماعي|أزواج|أطفال|أرطفونيا)$", ErrorMessage = "Session type must be either فردي or جماعيor أزواج or   أطفال or  أرطفونيا")]
        public string sessionType { get; set; } = string.Empty;
        public string? description { get; set; }
    }
}
