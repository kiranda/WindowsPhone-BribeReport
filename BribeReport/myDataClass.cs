using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BribeReport
{
    public class BribeData { 
        public string Id { get; set; }
        public string bribe_description { get; set; }
        public int bribe_amount { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string device_type { get; set; }
    
    }
}
