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

        public virtual DbSet<Blockage> Blockage { get; set; }
        public virtual DbSet<CheckListRef> CheckListRef { get; set; }
        public virtual DbSet<Conducteur> Conducteur { get; set; }
        public virtual DbSet<Engins> Engins { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Site> Site { get; set; }
        public virtual DbSet<Societe> Societe { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Vehicule> Vehicule { get; set; }
        public virtual DbSet<VM_GetCamionByStats> VM_GetCamionByStats { get; set; }

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
            modelBuilder.Entity<Blockage>(entity =>
            {
                entity.ToTable("blockage");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateBlockage)
                    .HasColumnName("dateBlockage")
                    .HasColumnType("date");

                entity.Property(e => e.DateDeblockage)
                    .HasColumnName("dateDeblockage")
                    .HasColumnType("date");

                entity.Property(e => e.IdCheckList)
                    .HasColumnName("idCheckList")
                    .IsUnicode(false);

                entity.Property(e => e.IdVehicule).HasColumnName("idVehicule");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("imageUrl")
                    .IsUnicode(false);

                entity.Property(e => e.Motif).HasColumnName("motif");

                entity.HasOne(d => d.IdVehiculeNavigation)
                    .WithMany(p => p.Blockage)
                    .HasForeignKey(d => d.IdVehicule)
                    .HasConstraintName("fk_idVehiculeBlockage");
            });

            modelBuilder.Entity<CheckListRef>(entity =>
            {
                entity.ToTable("checkListRef");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Etat).HasColumnName("etat");

                entity.Property(e => e.IdCheckListRef)
                    .HasColumnName("idCheckListRef")
                    .IsUnicode(false);

                entity.Property(e => e.IdConducteur).HasColumnName("idConducteur");

                entity.Property(e => e.IdSite).HasColumnName("idSite");

                entity.Property(e => e.IdVehicule).HasColumnName("idVehicule");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.HasOne(d => d.IdConducteurNavigation)
                    .WithMany(p => p.CheckListRef)
                    .HasForeignKey(d => d.IdConducteur)
                    .HasConstraintName("fk_idCond");

                entity.HasOne(d => d.IdSiteNavigation)
                    .WithMany(p => p.CheckListRef)
                    .HasForeignKey(d => d.IdSite)
                    .HasConstraintName("fk_idSiteCheckListRef");

                entity.HasOne(d => d.IdVehiculeNavigation)
                    .WithMany(p => p.CheckListRef)
                    .HasForeignKey(d => d.IdVehicule)
                    .HasConstraintName("fk_idVehicule");
            });

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
                    .HasConstraintName("FK__conducteu__idSoc__70DDC3D8");
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Libelle)
                    .HasColumnName("libelle")
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Site>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Libelle)
                    .HasColumnName("libelle")
                    .HasMaxLength(50);
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

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.IdSite).HasColumnName("idSite");

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

                entity.HasOne(d => d.IdSiteNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdSite)
                    .HasConstraintName("fk_idSite");

                entity.HasOne(d => d.IdroleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Idrole)
                    .HasConstraintName("FK__users__idrole__778AC167");
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
                    .HasConstraintName("FK__vehicule__idEngi__72C60C4A");
            });
            modelBuilder.Entity<VM_GetCamionByStats>(entity =>
            {
                entity.ToTable("VM_GetCamionByStats");

                entity.Property(e => e.libelle).HasColumnName("libelle");

                entity.Property(e => e.bloque).HasColumnName("bloque");

                entity.Property(e => e.Nonbloque).HasColumnName("Nonbloque");
            });

        }
    }
}
