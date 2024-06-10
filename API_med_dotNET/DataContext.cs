using API_med_dotNET.Models;
using Microsoft.EntityFrameworkCore;

namespace API_med_dotNET
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Products> Products { get; set; }
    }
}
