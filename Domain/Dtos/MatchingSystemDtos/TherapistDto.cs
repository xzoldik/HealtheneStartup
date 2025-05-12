using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.MatchingSystemDtos
{
    public class TherapistDto
    {
      
        public required int TherapistID { get; set; }
        public required int UserID { get; set; }
        public required string Specialization { get; set; }
        public required string Bio { get; set; }
        public required string Rating { get; set; }
        public required string Diploma { get; set; }
        public required string YearsOfExperience { get; set; }
          







    }
}
