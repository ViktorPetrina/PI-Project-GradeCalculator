using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GradeCalculator.Models;

public partial class PiGradeCalculatorContext : DbContext
{
    public PiGradeCalculatorContext()
    {
    }

    public PiGradeCalculatorContext(DbContextOptions<PiGradeCalculatorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Godina> Godinas { get; set; }

    public virtual DbSet<Korisnik> Korisniks { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Ocjena> Ocjenas { get; set; }

    public virtual DbSet<Predmet> Predmets { get; set; }

    public virtual DbSet<Uloga> Ulogas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:connection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Godina>(entity =>
        {
            entity.HasKey(e => e.Idgodina).HasName("PK__Godina__C94F992DC64DD7B5");

            entity.ToTable("Godina");

            entity.HasIndex(e => e.Naziv, "UQ__Godina__603E81468CA95397").IsUnique();

            entity.Property(e => e.Idgodina).HasColumnName("IDGodina");
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.Naziv).HasMaxLength(50);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Godinas)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK__Godina__Korisnik__2E1BDC42");
        });

        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.Idkorisnik).HasName("PK__Korisnik__6F9CD5C46AB4383E");

            entity.ToTable("Korisnik");

            entity.HasIndex(e => e.Eposta, "UQ__Korisnik__12BEDB67355A899C").IsUnique();

            entity.HasIndex(e => e.KorisnickoIme, "UQ__Korisnik__992E6F9217B98F2B").IsUnique();

            entity.Property(e => e.Idkorisnik).HasColumnName("IDKorisnik");
            entity.Property(e => e.Eposta)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EPosta");
            entity.Property(e => e.KorisnickoIme).HasMaxLength(50);
            entity.Property(e => e.LozinkaHash).HasMaxLength(255);
            entity.Property(e => e.LozinkaSalt).HasMaxLength(255);
            entity.Property(e => e.UlogaId).HasColumnName("UlogaID");

            entity.HasOne(d => d.Uloga).WithMany(p => p.Korisniks)
                .HasForeignKey(d => d.UlogaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Korisnik__UlogaI__2A4B4B5E");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Idocjena).HasName("PK__Log__F9D97F28DF5F4016");

            entity.ToTable("Log");

            entity.Property(e => e.Idocjena).HasColumnName("IDOcjena");
            entity.Property(e => e.Opis).HasMaxLength(100);
            entity.Property(e => e.Vrijeme).HasColumnType("datetime");
        });

        modelBuilder.Entity<Ocjena>(entity =>
        {
            entity.HasKey(e => e.Idocjena).HasName("PK__Ocjena__F9D97F2892D81898");

            entity.ToTable("Ocjena");

            entity.Property(e => e.Idocjena).HasColumnName("IDOcjena");
            entity.Property(e => e.PredmetId).HasColumnName("PredmetID");

            entity.HasOne(d => d.Predmet).WithMany(p => p.Ocjenas)
                .HasForeignKey(d => d.PredmetId)
                .HasConstraintName("FK__Ocjena__PredmetI__34C8D9D1");
        });

        modelBuilder.Entity<Predmet>(entity =>
        {
            entity.HasKey(e => e.Idpredmet).HasName("PK__Predmet__A6C7A1C4670C8642");

            entity.ToTable("Predmet");

            entity.HasIndex(e => e.Naziv, "UQ__Predmet__603E8146044002A0").IsUnique();

            entity.Property(e => e.Idpredmet).HasColumnName("IDPredmet");
            entity.Property(e => e.GodinaId).HasColumnName("GodinaID");
            entity.Property(e => e.Naziv).HasMaxLength(50);

            entity.HasOne(d => d.Godina).WithMany(p => p.Predmets)
                .HasForeignKey(d => d.GodinaId)
                .HasConstraintName("FK__Predmet__GodinaI__31EC6D26");
        });

        modelBuilder.Entity<Uloga>(entity =>
        {
            entity.HasKey(e => e.Iduloga).HasName("PK__Uloga__AB59C07D06B1F864");

            entity.ToTable("Uloga");

            entity.HasIndex(e => e.Naziv, "UQ__Uloga__603E8146525F5373").IsUnique();

            entity.Property(e => e.Iduloga).HasColumnName("IDUloga");
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
