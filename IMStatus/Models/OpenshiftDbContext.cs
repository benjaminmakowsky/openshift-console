using Microsoft.EntityFrameworkCore;

namespace IMStatus.Models;


public class OpenshiftDbContext: DbContext
{
    public DbSet<Project> Projects { get; set; }

    public OpenshiftDbContext(DbContextOptions<OpenshiftDbContext> options) : base(options)
    {
        
    }
}