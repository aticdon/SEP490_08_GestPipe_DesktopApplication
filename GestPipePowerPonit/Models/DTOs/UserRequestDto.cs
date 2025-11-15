using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models.DTOs
{
    public class UserRequestDto
    {
        public string Id { get; set; }

        public string GestureRequestStatus { get; set; }
        public int RequestCountToday { get; set; }
    }
}
