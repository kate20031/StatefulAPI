using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StatefulAPI.Models.EntityFramework;

public partial class SeriesDbContext : DbContext
{
    public SeriesDbContext()
    {
    }

    public SeriesDbContext(DbContextOptions<SeriesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Serie> Series { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MonNomDeBase;Username=MonUtilisateur;Password=MonMotDePasse");
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Serie>(entity =>
        {
            entity.HasKey(e => e.Serieid).HasName("serie_pkey");

            entity.ToTable("serie");

            entity.Property(e => e.Serieid).HasColumnName("serieid");
            entity.Property(e => e.Anneecreation).HasColumnName("anneecreation");
            entity.Property(e => e.Nbepisodes).HasColumnName("nbepisodes");
            entity.Property(e => e.Nbsaisons).HasColumnName("nbsaisons");
            entity.Property(e => e.Network)
                .HasMaxLength(50)
                .HasColumnName("network");
            entity.Property(e => e.Resume).HasColumnName("resume");
            entity.Property(e => e.Titre)
                .HasMaxLength(100)
                .HasColumnName("titre");
        });

        OnModelCreatingPartial(modelBuilder);
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
