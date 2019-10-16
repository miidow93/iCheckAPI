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
        public virtual DbSet<Vehicule> Vehicule { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=iCheck; Integrated Security=True;");
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

                entity.Property(e => e.IdSociete).HasColumnName("idSociete");

                entity.Property(e => e.NomComplet)
                    .HasColumnName("nomComplet")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Patente)
                    .HasColumnName("patente")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdSocieteNavigation)
                    .WithMany(p => p.Conducteur)
                    .HasForeignKey(d => d.IdSociete)
                    .HasConstraintName("FK__conducteu__idsoc__4D94879B");
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
                entity.ToTable("societe");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Libelle)
                    .HasColumnName("libelle")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Vehicule>(entity =>
            {
                entity.ToTable("vehicule");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdEngin).HasColumnName("idEngin");

                entity.Property(e => e.Matricule)
                    .HasColumnName("matricule")
                    .HasMaxLength(30);

                entity.HasOne(d => d.IdEnginNavigation)
                    .WithMany(p => p.Vehicule)
                    .HasForeignKey(d => d.IdEngin)
                    .HasConstraintName("FK__vehicule__idEngi__5FB337D6");
            });
        }
    }
}
