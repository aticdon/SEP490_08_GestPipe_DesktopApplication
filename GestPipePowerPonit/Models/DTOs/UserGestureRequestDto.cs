using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models.DTOs
{
    public class UserGestureRequestDto
    {
        public string UserId { get; set; }
        public string UserGestureConfigId { get; set; }
        public string GestureTypeId { get; set; }
        public string PoseLabel { get; set; }
        public Dictionary<string, string> Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
