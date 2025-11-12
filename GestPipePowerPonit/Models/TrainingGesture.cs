using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class TrainingGesture
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PoseLabel { get; set; }
        public int TotalTrain { get; set; }
        public int CorrectTrain { get; set; }
        public double Accuracy { get; set; }
        public VectorData VectorData { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
