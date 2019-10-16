using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace iCheckAPI.Models
{
    public partial class icheckContext : DbContext
    {
        public icheckContext()
        {
        }

        public icheckContext(DbContextOptions<icheckContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Conducteur> Conducteur { get; set; }
        public virtual DbSet<Engins> Engins { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Societe> Societe { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=icheck;Trusted_Connection=True;");
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

                entity.HasOne(d => d.IdsocieteNavigation)
                    .WithMany(p => p.Conducteur)
                    .HasForeignKey(d => d.IdSociete)
                    .HasConstraintName("FK__conducteu__idsoc__5165187F");
            });

            modelBuilder.Entity<Engins>(entity =>
            {
                entity.ToTable("engins");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageEngin)
                    .HasColumnName("imageEngin")
                    .IsUnicode(false);

                entity.Property(e => e.Matricule)
                    .HasColumnName("matricule")
                    .IsUnicode(false);

                entity.Property(e => e.NomEngin)
                    .HasColumnName("nomEngin")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Idrole);

                entity.ToTable("role");

                entity.Property(e => e.Idrole).HasColumnName("idrole");

                entity.Property(e => e.Role1)
                    .HasColumnName("role")
                    .HasMaxLength(80)
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

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Idrole).HasColumnName("idrole");

                entity.Property(e => e.NomComplet)
                    .HasColumnName("nomComplet")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdroleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Idrole)
                    .HasConstraintName("FK__users__idrole__52593CB8");
            });
        }
    }
}
