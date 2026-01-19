using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class Session
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        public string TopicId { get; set; }
        public Dictionary<string, int> Records { get; set; }
        public double Duration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
