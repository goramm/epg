using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPGWPF
{
    class Programme
    {
        public Programme()
        {
        }
        public Channel Channel { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return String.Format("{0:HH:mm} - {1}", Start, Title);
        }
    }
}
