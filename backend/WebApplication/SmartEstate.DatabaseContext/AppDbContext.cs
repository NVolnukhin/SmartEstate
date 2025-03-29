
using DatabaseModel;
using Microsoft.EntityFrameworkCore;

namespace DatabaseContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<InfrastructureInfo> InfrastructureInfos { get; set; }
    public DbSet<Developer> Developers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Flat> Flats { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<UserComparison> UserComparisons { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Building - Developer
        modelBuilder.Entity<Building>()
            .HasOne(b => b.Developer)
            .WithMany(d => d.Buildings)
            .HasForeignKey(b => b.DeveloperId);
            
        // Building - InfrastructureInfo
        modelBuilder.Entity<Building>()
            .HasOne(b => b.InfrastructureInfo)
            .WithOne(i => i.Building)
            .HasForeignKey<Building>(b => b.InfrastructureInfoId);
            
        // Flat - Building
        modelBuilder.Entity<Flat>()
            .HasOne(f => f.Building)
            .WithMany(b => b.Flats)
            .HasForeignKey(f => f.BuildingId);
            
        // PriceHistory - Flat
        modelBuilder.Entity<PriceHistory>()
            .HasOne(p => p.Flat)
            .WithMany(f => f.PriceHistories)
            .HasForeignKey(p => p.FlatId);
        
        // UserFavorite - User & Flat
        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(uf => uf.FavoriteId);
        
            entity.HasOne(uf => uf.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(uf => uf.UserId);
            
            entity.HasOne(uf => uf.Flat)
                .WithMany(f => f.Favorites)
                .HasForeignKey(uf => uf.FlatId);
        });
        
        // UserComparison - User & Flats
        modelBuilder.Entity<UserComparison>(entity =>
        {
            entity.HasKey(uc => uc.CompareId);
        
            entity.HasOne(uc => uc.User)
                .WithMany(u => u.Comparisons)
                .HasForeignKey(uc => uc.UserId);
            
            entity.HasOne(uc => uc.FirstFlat)
                .WithMany(f => f.ComparisonsAsFirst)
                .HasForeignKey(uc => uc.FlatId1)
                .OnDelete(DeleteBehavior.NoAction);
            
            entity.HasOne(uc => uc.SecondFlat)
                .WithMany(f => f.ComparisonsAsSecond)
                .HasForeignKey(uc => uc.FlatId2)
                .OnDelete(DeleteBehavior.NoAction);
        });

    }
}