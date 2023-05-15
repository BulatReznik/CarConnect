using Microsoft.EntityFrameworkCore;
using CarConnectDataBase.Models;

namespace CarConnectDataBase
{
    public class CarConnectDataBase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer(@"Data Source=LAPTOP-UJ3H26AC\SQLEXPRESS;Initial Catalog=CarConnectDataBase;Integrated Security=True; MultipleActiveResultSets=True; TrustServerCertificate=Yes");
            }
            base.OnConfiguring(optionsBuilder);
        }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<User> Users { get; set; }

    }
}
