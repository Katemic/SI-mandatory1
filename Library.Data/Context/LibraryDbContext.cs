using System;
using System.Collections.Generic;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Context;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tauthor> Tauthors { get; set; }

    public virtual DbSet<Tbook> Tbooks { get; set; }

    public virtual DbSet<Tpublishingcompany> Tpublishingcompanies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Tauthor>(entity =>
        {
            entity.HasKey(e => e.NAuthorId).HasName("PRIMARY");

            entity.ToTable("tauthor");

            entity.Property(e => e.NAuthorId).HasColumnName("nAuthorID");
            entity.Property(e => e.CName)
                .HasMaxLength(40)
                .HasColumnName("cName");
            entity.Property(e => e.CSurname)
                .HasMaxLength(60)
                .HasColumnName("cSurname");
        });

        modelBuilder.Entity<Tbook>(entity =>
        {
            entity.HasKey(e => e.NBookId).HasName("PRIMARY");

            entity.ToTable("tbook");

            entity.HasIndex(e => e.NAuthorId, "fk_book_author");

            entity.HasIndex(e => e.NPublishingCompanyId, "fk_book_publishing_company");

            entity.Property(e => e.NBookId).HasColumnName("nBookID");
            entity.Property(e => e.CTitle)
                .HasMaxLength(255)
                .HasColumnName("cTitle");
            entity.Property(e => e.NAuthorId).HasColumnName("nAuthorID");
            entity.Property(e => e.NPublishingCompanyId).HasColumnName("nPublishingCompanyID");
            entity.Property(e => e.NPublishingYear).HasColumnName("nPublishingYear");

            entity.HasOne(d => d.NAuthor).WithMany(p => p.Tbooks)
                .HasForeignKey(d => d.NAuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_book_author");

            entity.HasOne(d => d.NPublishingCompany).WithMany(p => p.Tbooks)
                .HasForeignKey(d => d.NPublishingCompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_book_publishing_company");
        });

        modelBuilder.Entity<Tpublishingcompany>(entity =>
        {
            entity.HasKey(e => e.NPublishingCompanyId).HasName("PRIMARY");

            entity.ToTable("tpublishingcompany");

            entity.Property(e => e.NPublishingCompanyId).HasColumnName("nPublishingCompanyID");
            entity.Property(e => e.CName)
                .HasMaxLength(40)
                .HasColumnName("cName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
