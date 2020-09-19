using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace EPGWPF
{
    class EPG
    {
        public EPG()
        {
        }

        public List<Channel> Channels { get; set; }
        public List<Programme> Programmes { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<Category> Categories { get; set; }
    }
}
