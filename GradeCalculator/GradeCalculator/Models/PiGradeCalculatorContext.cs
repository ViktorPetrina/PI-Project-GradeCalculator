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
        modelBuilder.UseCollation("Croatian_CI_AS");

        modelBuilder.Entity<Godina>(entity =>
        {
            entity.HasKey(e => e.Idgodina).HasName("PK__Godina__C94F992DAA6DBB9E");

            entity.ToTable("Godina");

            entity.Property(e => e.Idgodina).HasColumnName("IDGodina");
            entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");
            entity.Property(e => e.Naziv).HasMaxLength(50);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Godinas)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK__Godina__Korisnik__70DDC3D8");
        });

        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.Idkorisnik).HasName("PK__Korisnik__6F9CD5C41D1ADFE6");

            entity.ToTable("Korisnik");

            entity.HasIndex(e => e.Eposta, "UQ__Korisnik__12BEDB6790A4B903").IsUnique();

            entity.HasIndex(e => e.KorisnickoIme, "UQ__Korisnik__992E6F92A2974909").IsUnique();

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
            entity.HasKey(e => e.Idlog).HasName("PK__Log__95D002080CBA8D0A");

            entity.ToTable("Log");

            entity.Property(e => e.Idlog).HasColumnName("IDLog");
            entity.Property(e => e.Opis).HasMaxLength(100);
            entity.Property(e => e.Vrijeme).HasColumnType("datetime");
        });

        modelBuilder.Entity<Ocjena>(entity =>
        {
            entity.HasKey(e => e.Idocjena).HasName("PK__Ocjena__F9D97F2811524605");

            entity.ToTable("Ocjena");

            entity.Property(e => e.Idocjena).HasColumnName("IDOcjena");
            entity.Property(e => e.PredmetId).HasColumnName("PredmetID");

            entity.HasOne(d => d.Predmet).WithMany(p => p.Ocjenas)
                .HasForeignKey(d => d.PredmetId)
                .HasConstraintName("FK__Ocjena__PredmetI__76969D2E");
        });

        modelBuilder.Entity<Predmet>(entity =>
        {
            entity.HasKey(e => e.Idpredmet).HasName("PK__Predmet__A6C7A1C4BCBD86A6");

            entity.ToTable("Predmet");

            entity.Property(e => e.Idpredmet).HasColumnName("IDPredmet");
            entity.Property(e => e.GodinaId).HasColumnName("GodinaID");
            entity.Property(e => e.Naziv).HasMaxLength(50);

            entity.HasOne(d => d.Godina).WithMany(p => p.Predmets)
                .HasForeignKey(d => d.GodinaId)
                .HasConstraintName("FK__Predmet__GodinaI__73BA3083");
        });

        modelBuilder.Entity<Uloga>(entity =>
        {
            entity.HasKey(e => e.Iduloga).HasName("PK__Uloga__AB59C07D001A01F3");

            entity.ToTable("Uloga");

            entity.HasIndex(e => e.Naziv, "UQ__Uloga__603E81468A0251B5").IsUnique();

            entity.Property(e => e.Iduloga).HasColumnName("IDUloga");
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
