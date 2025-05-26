using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.FeedbackDtos
{
    public class GetFeedbackDTO
    {
        public int FeedbackId { get; set; }
        public int SessionId { get; set; }
        public string SessionType { get; set; } = string.Empty; 
        public int patientID { get; set; }
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } =   string.Empty ;
        public int TherapistID { get; set; }
        public string TherapistFirstName { get; set; } = string.Empty ;
        public string TherapistLastName { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public int Rating { get; set; }



    }
}
