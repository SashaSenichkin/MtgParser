#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;

namespace ImageService.Context;

/// <summary>
/// images part of full db context 
/// </summary>
public class MtgContext : DbContext
{
    /// <summary>
    /// init db context, used in tests
    /// </summary>
    /// <param name="options"> </param>
    public MtgContext(DbContextOptions options):base(options)
    {}
    
    /// <summary>
    /// all sets, that figure in our Cards
    /// </summary>
    public DbSet<Set> Sets { get; set; }
    
    /// <summary>
    /// Card essence) can be in many sets, various rarities, etc
    /// </summary>
    public DbSet<Card> Cards { get; set; }

}