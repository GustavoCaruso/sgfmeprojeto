using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SGFME.Infrastructure.Data.Context
{
    public class SqlServerContextFactory : IDesignTimeDbContextFactory<SqlServerContext>
    {
        public SqlServerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
            var connectionString = @"Server=sql8005.site4now.net;DataBase=db_aa9649_sgfme;User Id=db_aa9649_sgfme_admin;Password=Gu_448228;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new SqlServerContext(optionsBuilder.Options);
        }
    }
}
