using Microsoft.EntityFrameworkCore;
using MtgParser.Model;

namespace MtgParser.Context;

public class MtgContext : DbContext
{
    public MtgContext(DbContextOptions options):base(options)
    {}
    
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Rarity> Rarities { get; set; }
    public DbSet<Set> Sets { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardSet> CardsSets { get; set; }
    public DbSet<CardName> CardsNames { get; set; }
}