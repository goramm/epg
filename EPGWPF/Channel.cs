using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPGWPF
{
    class Channel
    {
        public Channel() {}
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public List<Programme> Programmes { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
