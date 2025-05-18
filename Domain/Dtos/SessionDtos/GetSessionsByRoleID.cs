using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.SessionDtos
{
    public  class GetSessionsByRoleID
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; } = string.Empty;
        public IEnumerable<SessionDataModel>?  sessions { get; set; }
}
}
