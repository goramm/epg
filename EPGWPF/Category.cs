using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPGWPF
{
    class Category
    {
        public Category() { }
        public string Name { get; set; }

        public string[] Patterns { get; set; }
    }
}
