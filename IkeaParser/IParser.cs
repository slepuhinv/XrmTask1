using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IkeaParser
{
    public interface IParser
    {
        void Parse();
        Task ParseAsync(IProgress<ParseProgress> progress);

        List<Department> Departments { get; }
        List<SubCategory> SubCategories { get; }
        List<Product> Products { get; }
    }
}
