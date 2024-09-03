using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SGFME.Infrastructure.Data.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SqlServerContext>
    {
        public SqlServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
            optionsBuilder.UseSqlServer("Data Source=SQL8020.site4now.net;Initial Catalog=db_aa9649_ggustac;User Id=db_aa9649_ggustac_admin;Password=sgfme123456;");


            return new SqlServerContext(optionsBuilder.Options);
        }
    }
}
