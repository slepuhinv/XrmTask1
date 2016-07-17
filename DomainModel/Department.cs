using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Department
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public virtual List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Department d = obj as Department;
            if (d == null)
            {
                return false;
            }
            return Id == d.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
