using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheque.Models;

public partial class BibliothequeContext : IdentityDbContext<User>
{
    public BibliothequeContext()
    {
    }

    public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Avis> Avis { get; set; }

    public virtual DbSet<Emprunt> Emprunts { get; set; }

    public virtual DbSet<Etudiant> Etudiants { get; set; }

    public virtual DbSet<Livre> Livres { get; set; }

    public virtual DbSet<Motcle> Motcles { get; set; }

    public virtual DbSet<Visite> Visites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // 🔴 THIS LINE FIXES EVERYTHING

        modelBuilder.Entity<Avis>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_avis_id");

            entity.ToTable("avis");

            entity.HasIndex(e => e.Cin, "avis_cin_foreign");

            entity.HasIndex(e => e.Numinv, "avis_numinv_foreign");

            entity.HasIndex(e => new { e.Cin, e.Numinv }, "avis_cin_numinv_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(20, 0)")
                .HasColumnName("id");
            entity.Property(e => e.Cin)
                .HasMaxLength(255)
                .HasColumnName("cin");
            entity.Property(e => e.Numinv)
                .HasMaxLength(255)
                .HasColumnName("numinv");
            entity.Property(e => e.Note)
                .HasColumnName("note");
            entity.Property(e => e.Commentaire)
                .HasMaxLength(500)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("commentaire");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Etudiant).WithMany(p => p.Avis)
                .HasForeignKey(d => d.Cin)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("avis$avis_cin_foreign");

            entity.HasOne(d => d.Livre).WithMany(p => p.Avis)
                .HasForeignKey(d => d.Numinv)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("avis$avis_numinv_foreign");
        });

        modelBuilder.Entity<Emprunt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_emprunts_id");

            entity.ToTable("emprunts");

            entity.HasIndex(e => e.Cin, "emprunts_cin_foreign");

            entity.HasIndex(e => e.Numinv, "emprunts_numinv_foreign");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(20, 0)")
                .HasColumnName("id");
            entity.Property(e => e.Cin)
                .HasMaxLength(255)
                .HasColumnName("cin");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dateemprunt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("dateemprunt");
            entity.Property(e => e.Dateretour)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("dateretour");
            entity.Property(e => e.Estretour)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("estretour");
            entity.Property(e => e.Numinv)
                .HasMaxLength(255)
                .HasColumnName("numinv");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CinNavigation).WithMany(p => p.Emprunts)
                .HasForeignKey(d => d.Cin)
                .HasConstraintName("emprunts$emprunts_cin_foreign");

            entity.HasOne(d => d.NuminvNavigation).WithMany(p => p.Emprunts)
                .HasForeignKey(d => d.Numinv)
                .HasConstraintName("emprunts$emprunts_numinv_foreign");
        });

        modelBuilder.Entity<Etudiant>(entity =>
        {
            entity.HasKey(e => e.Cin).HasName("PK_etudiants_cin");

            entity.ToTable("etudiants");

            entity.HasIndex(e => e.Email, "etudiants$etudiants_email_unique").IsUnique();

            entity.HasIndex(e => e.Matricule, "etudiants$etudiants_matricule_unique").IsUnique();

            entity.HasIndex(e => e.Qr, "etudiants$etudiants_qr_unique").IsUnique();

            entity.HasIndex(e => e.Rfid, "etudiants$etudiants_rfid_unique").IsUnique();

            entity.Property(e => e.Cin)
                .HasMaxLength(255)
                .HasColumnName("cin");
            entity.Property(e => e.Commentaire)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("commentaire");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("email");
            entity.Property(e => e.Filiere)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("filiere");
            entity.Property(e => e.Matricule)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("matricule");
            entity.Property(e => e.Niveau)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nom)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("nom");
            entity.Property(e => e.Observation)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("observation");
            entity.Property(e => e.Prenom)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("prenom");
            entity.Property(e => e.Qr)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("qr");
            entity.Property(e => e.Rfid)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("rfid");
            entity.Property(e => e.Tel)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("tel");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Livre>(entity =>
        {
            entity.HasKey(e => e.Numinventaire).HasName("PK_livres_numinventaire");

            entity.ToTable("livres");

            entity.Property(e => e.Numinventaire)
                .HasMaxLength(255)
                .HasColumnName("numinventaire");
            entity.Property(e => e.Auteur)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("auteur");
            entity.Property(e => e.Cote)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("cote");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Datearrivage)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("datearrivage");
            entity.Property(e => e.Datepublication)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("datepublication");
            entity.Property(e => e.Editeur)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("editeur");
            entity.Property(e => e.Isbn)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("isbn");
            entity.Property(e => e.Résumé)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("résumé");
            entity.Property(e => e.Titre)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("titre");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Motcle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_motcles_id");

            entity.ToTable("motcles");

            entity.HasIndex(e => e.Numinv, "motcles_numinv_foreign");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(20, 0)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Mot)
                .HasMaxLength(255)
                .HasColumnName("mot");
            entity.Property(e => e.Numinv)
                .HasMaxLength(255)
                .HasColumnName("numinv");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.NuminvNavigation).WithMany(p => p.Motcles)
                .HasForeignKey(d => d.Numinv)
                .HasConstraintName("motcles$motcles_numinv_foreign");
        });

        modelBuilder.Entity<Visite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_visites_id");

            entity.ToTable("visites");

            entity.HasIndex(e => e.Cin, "visites_cin_foreign");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cin)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("CIN");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CinNavigation).WithMany(p => p.Visites)
                .HasForeignKey(d => d.Cin)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("visites$visites_cin_foreign");
        });

        // your existing scaffolded config
        OnModelCreatingPartial(modelBuilder);

    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
