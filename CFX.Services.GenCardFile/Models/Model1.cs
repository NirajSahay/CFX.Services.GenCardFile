using System;
using System.Collections.Generic;
using System.Text;

namespace CFX.Services.GenCardFile.Models
{
    public class Model1
    {
        public string Id { get; set; }
       public string file_transfer_id { get; set; }
        public string file_type { get; set; }
        public string file_name_out { get; set; }
        public DateTime file_date_out { get; set; }
        public int record_count_out { get; set; }
        public int total_amount_out { get; set; }
        public string file_name_in { get; set; }
        public DateTime file_date_in { get; set; }
        public int record_count_in { get; set; }
        public int total_amount_in { get; set; }
        public int error_count_in { get; set; }
    }
}
