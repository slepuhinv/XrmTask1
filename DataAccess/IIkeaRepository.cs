using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IIkeaRepository : IDisposable
    {
        List<Department> GetDepartments();
        List<SubCategory> GetSubCategories();
        List<Product> GetProducts();

        void AddDepartments(List<Department> departments);
        void AddSubCategories(List<SubCategory> subCategories);
        void AddProducts(List<Product> products);

        void Clear();
        void Save();
    }
}
