using Microsoft.EntityFrameworkCore;

namespace ImageService.Context;

public class MtgContext : DbContext
{
    public MtgContext(DbContextOptions options):base(options)
    {}
    
    public DbSet<Set> Sets { get; set; }
    public DbSet<Card> Cards { get; set; }

}