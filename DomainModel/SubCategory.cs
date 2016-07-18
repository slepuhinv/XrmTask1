using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SubCategory
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Department Department { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}
