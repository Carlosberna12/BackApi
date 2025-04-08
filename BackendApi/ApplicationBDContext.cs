using BackendApi.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackendApi
{
    public class ApplicationBDContext : IdentityDbContext
    {
        public ApplicationBDContext(DbContextOptions<ApplicationBDContext> options) : base(options)
        {
        }

        public DbSet<Empleados> Empleados { get; set; }
    }
}
