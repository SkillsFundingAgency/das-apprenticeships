using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Apprenticeships.DataAccess.Entities;

namespace SFA.DAS.Apprenticeships.DataAccess
{
    public class ApprenticeshipsDataContext : DbContext
    {
        private readonly string _connectionString;

        public ApprenticeshipsDataContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ApprenticeshipsDatabase");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public virtual DbSet<Earning> Earning { get; set; }
    }
}
