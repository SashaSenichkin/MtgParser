using Microsoft.EntityFrameworkCore;
using MtgParser.Model;

namespace MtgParser.Context;

/// <summary>
/// Full Db context for migrations.. have to be moved to separate project
/// </summary>
public class MtgContext : DbContext
{
    /// <summary>
    /// context ctor for test and migrations auto-apply
    /// </summary>
    /// <param name="options"></param>
    public MtgContext(DbContextOptions options):base(options)
    {}
    
    /// <summary>
    /// basic mtg Keywors
    /// </summary>
    public DbSet<Keyword> Keywords { get; set; }
    
    /// <summary>
    /// auto parsed prices. have to move all here soon
    /// </summary>
    public DbSet<Price> Prices { get; set; }
    
    /// <summary>
    /// enumeration for 5 rarity values
    /// </summary>
    public DbSet<Rarity> Rarities { get; set; }
    
    /// <summary>
    /// all sets, that figure in our Cards
    /// </summary>
    public DbSet<Set> Sets { get; set; }
    
    /// <summary>
    /// Card essence) can be in many sets, various rarities, etc
    /// </summary>
    public DbSet<Card> Cards { get; set; }
    
    /// <summary>
    /// physical embodiment of card 
    /// </summary>
    public DbSet<CardSet> CardsSets { get; set; }
    
    /// <summary>
    /// list of cards for mass parsing
    /// </summary>
    public DbSet<CardName> CardsNames { get; set; }
    
    /// <summary>
    /// list of our users
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// role-model part
    /// </summary>
    public DbSet<Permission> Permissions { get; set; }
    
    /// <summary>
    /// card orders
    /// </summary>
    public DbSet<Order> Orders { get; set; }


    /// <summary>
    /// enumerations auto fill 
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Rarity>().HasData
        (
            new Rarity { Id = 1, Name = "Common", RusName = "Обычная"},
            new Rarity { Id = 2, Name = "Uncommon", RusName = "Необычная"},
            new Rarity { Id = 3, Name = "Rare", RusName = "Редкая"},
            new Rarity { Id = 4, Name = "Mythic", RusName = "Раритетная"},
            new Rarity { Id = 5, Name = "Special", RusName = "Специальная"}
        );
        
        modelBuilder.Entity<Keyword>().HasData
        (
            new Keyword { Id = 1, Name = "Deathtouch", RusName = "Смертельное касание"},
            new Keyword { Id = 2, Name = "Defender", RusName = "Защитник"},
            new Keyword { Id = 3, Name = "Double strike", RusName = "Двойной удар"},
            new Keyword { Id = 4, Name = "Enchant", RusName = "Зачаровать"},
            new Keyword { Id = 5, Name = "Equip", RusName = "Снарядить"},
            new Keyword { Id = 6, Name = "First strike", RusName = "Первый удар"},
            new Keyword { Id = 7, Name = "Flash", RusName = "Миг"},
            new Keyword { Id = 8, Name = "Flying", RusName = "Полет"},
            new Keyword { Id = 9, Name = "Haste", RusName = "Ускорение"},
            new Keyword { Id = 10, Name = "Hexproof", RusName = "Порчеустойчивость"},
            new Keyword { Id = 11, Name = "Indestructible", RusName = "Неразрушимость"},
            new Keyword { Id = 12, Name = "Lifelink", RusName = "Цепь жизни"},
            new Keyword { Id = 13, Name = "Menace", RusName = "Угроза"},
            new Keyword { Id = 14, Name = "Protection", RusName = "Защита"},
            new Keyword { Id = 15, Name = "Prowess", RusName = "Искусность"},
            new Keyword { Id = 16, Name = "Reach", RusName = "Захват"},
            new Keyword { Id = 17, Name = "Trample", RusName = "Пробивной удар"},
            new Keyword { Id = 18, Name = "Vigilance", RusName = "Бдительность"}
        );
    }
}