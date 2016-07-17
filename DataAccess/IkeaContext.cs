using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class IkeaContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<Product> Products { get; set; }

        public IkeaContext()
            : base ("IkeaDbConnection")
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;

            Database.SetInitializer<IkeaContext>(new DropCreateDatabaseIfModelChanges<IkeaContext>());
        }
    }
}
