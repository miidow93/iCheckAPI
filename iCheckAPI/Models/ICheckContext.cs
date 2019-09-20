using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace iCheckAPI.Models
{
    public partial class ICheckContext : DbContext
    {
        public ICheckContext()
        {
        }

        public ICheckContext(DbContextOptions<ICheckContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Conducteur> Conducteur { get; set; }
        public virtual DbSet<Engins> Engins { get; set; }
        public virtual DbSet<Societe> Societe { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=iCheck;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conducteur>(entity =>
            {
                entity.ToTable("conducteur");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Assurance)
                    .HasColumnName("assurance")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Cin)
                    .HasColumnName("cin")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cnss)
                    .HasColumnName("cnss")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.DateValiditeAssurance)
                    .HasColumnName("dateValiditeAssurance")
                    .HasColumnType("date");

                entity.Property(e => e.IdSociete).HasColumnName("idsociete");

                entity.Property(e => e.NomComplet)
                    .HasColumnName("nomcomplet")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Patente)
                    .HasColumnName("patente")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSocieteNavigation)
                    .WithMany(p => p.Conducteur)
                    .HasForeignKey(d => d.IdSociete)
                    .HasConstraintName("FK__conducteu__idsoc__3B75D760");
            });

            modelBuilder.Entity<Engins>(entity =>
            {
                entity.ToTable("engins");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageEngin)
                    .HasColumnName("imageEngin")
                    .IsUnicode(false);

                entity.Property(e => e.NomEngin)
                    .HasColumnName("nomEngin")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Societe>(entity =>
            {
                entity.HasKey(e => e.IdSociete);

                entity.ToTable("societe");

                entity.Property(e => e.IdSociete).HasColumnName("idsociete");

                entity.Property(e => e.Libelle)
                    .HasColumnName("libelle")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
