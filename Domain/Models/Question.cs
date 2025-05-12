using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Question
    {
        public int? QuestionID { get; set; }
        public int? QuestionnaireID { get; set; }
        public required string QuestionContent { get; set; } = string.Empty;
        public required int QuestionOrder { get; set; }
        public required List<Answer> Answers { get; set; } = new List<Answer>();

    }
}

