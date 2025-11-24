using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models.DTOs
{
    public class ImportGestureFromCsvRequest
    {
        public string UserId { get; set; }
        public string CsvContent { get; set; }
    }
}
