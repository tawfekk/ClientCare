using Microsoft.EntityFrameworkCore;
using ClientCare.Models;

namespace ClientCare.Data
{
    public partial class CRMContext
    {
        partial void OnModelBuilding(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        }
    }
}