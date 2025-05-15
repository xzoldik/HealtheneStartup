using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Questionnaire
    {
        public int QuestionnaireID { get; set; }
        public required string QuestionnaireTitle { get; set; }
        public required string QuestionnaireDescription { get; set; }
        public required  List<Question> Questions { get; set; }
    }
}
