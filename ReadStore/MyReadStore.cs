using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadStore
{
    public class MyReadStore : DbContext
    {

        public MyReadStore() : base("MyReadStore")
        {

        }

        public DbSet<SomeItem> SomeItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

    }

    public class SomeItem
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
    }

}
