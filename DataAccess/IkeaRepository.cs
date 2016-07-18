using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DataAccess
{
    public class IkeaRepository : IIkeaRepository
    {
        private IkeaContext context = new IkeaContext();

        public List<Department> GetDepartments()
        {
            return context.Departments.ToList();
        }

        public List<SubCategory> GetSubCategories()
        {
            return context.SubCategories.ToList();
        }

        public List<Product> GetProducts()
        {
            return context.Products.ToList();
        }

        public void AddDepartments(List<Department> departments)
        {
            context.Departments.AddRange(departments);
        }

        public void AddSubCategories(List<SubCategory> subCategories)
        {
            context.SubCategories.AddRange(subCategories);
        }

        public void AddProducts(List<Product> products)
        {
            context.Products.AddRange(products);
        }

        public void Clear()
        {
            context.Departments.RemoveRange(context.Departments);
            context.SubCategories.RemoveRange(context.SubCategories);
            context.Products.RemoveRange(context.Products);
        }
        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (context != null) context.Dispose();
                }
            }
            disposed = true;
        }

    }
}
