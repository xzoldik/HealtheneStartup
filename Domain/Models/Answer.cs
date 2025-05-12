using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Answer
    {
        public  int? AnswerID { get; set; }
        public  int? QuestionID { get; set; }
        public required string OptionText { get; set; } = string.Empty;
        public int? Score { get; set; }

    }
}
