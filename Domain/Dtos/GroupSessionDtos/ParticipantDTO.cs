using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.GroupSessionDtos
{
    public class ParticipantDTO
    {
        
        public int PatientID { get; set; }
        public string PatientFirstName { get; set; } = string.Empty;    
        public string PatientLastName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}
