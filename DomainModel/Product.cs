using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string ArticleNumber { get; set; }

        public string Price { get; set; }

        public string Image { get; set; }

        public virtual List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Product p = obj as Product;
            if (p == null)
            {
                return false;
            }
            return Id == p.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
