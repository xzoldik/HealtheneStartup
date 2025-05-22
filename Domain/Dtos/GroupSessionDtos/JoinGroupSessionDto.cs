using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.GroupSessionDtos
{
    public class JoinGroupSessionDto
    {
        public bool success { get; set; }
        public int returncode { get; set; }
        public string message { get; set; } = string.Empty;

    }
}
