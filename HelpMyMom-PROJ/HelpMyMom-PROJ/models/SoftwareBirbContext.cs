using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HelpMyMom_PROJ.models;

public partial class SoftwareBirbContext : DbContext
{
    public SoftwareBirbContext()
    {
    }

    public SoftwareBirbContext(DbContextOptions<SoftwareBirbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<ChatLog> ChatLogs { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Helper> Helpers { get; set; }

    public virtual DbSet<Mother> Mothers { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Spec> Specs { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=will-software-sql.database.windows.net;Database=SoftwareBirb;User Id=dbadmin;Password=$Egg420gryph;Encrypt=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Username);

            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
            entity.Property(e => e.ChildId).HasColumnName("childID");
            entity.Property(e => e.HelperId).HasColumnName("helperID");
            entity.Property(e => e.MomId).HasColumnName("momID");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
        });

        modelBuilder.Entity<ChatLog>(entity =>
        {
            entity.ToTable("ChatLog");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Image)
                .HasColumnType("image")
                .HasColumnName("image");
            entity.Property(e => e.IsMom)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("isMom");
            entity.Property(e => e.Text)
                .HasMaxLength(2000)
                .HasColumnName("text");
            entity.Property(e => e.TicketId).HasColumnName("ticketId");
            entity.Property(e => e.Time)
                .HasColumnType("datetime")
                .HasColumnName("time");

            entity.HasOne(d => d.Ticket).WithMany(p => p.ChatLogs)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("ticketLink");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FName)
                .HasMaxLength(35)
                .HasColumnName("fName");
            entity.Property(e => e.LName)
                .HasMaxLength(35)
                .HasColumnName("lName");
        });

        modelBuilder.Entity<Helper>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Banned).HasColumnName("banned");
            entity.Property(e => e.Description)
                .HasMaxLength(800)
                .HasColumnName("description");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FName)
                .HasMaxLength(35)
                .HasColumnName("fName");
            entity.Property(e => e.LName)
                .HasMaxLength(35)
                .HasColumnName("lName");
            entity.Property(e => e.Pfp)
                .HasColumnType("image")
                .HasColumnName("pfp");
            entity.Property(e => e.Specs)
                .HasMaxLength(300)
                .HasColumnName("specs");
            entity.Property(e => e.Tokens).HasColumnName("tokens");
        });

        modelBuilder.Entity<Mother>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FName)
                .HasMaxLength(35)
                .HasColumnName("fName");
            entity.Property(e => e.LName)
                .HasMaxLength(35)
                .HasColumnName("lName");
            entity.Property(e => e.Tokens).HasColumnName("tokens");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChildId).HasColumnName("childId");
            entity.Property(e => e.MomId).HasColumnName("momId");

            entity.HasOne(d => d.Child).WithMany(p => p.Relationships)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("children1");

            entity.HasOne(d => d.Mom).WithMany(p => p.Relationships)
                .HasForeignKey(d => d.MomId)
                .HasConstraintName("Moms");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Body)
                .HasMaxLength(2000)
                .HasColumnName("body");
            entity.Property(e => e.ChildId).HasColumnName("childId");
            entity.Property(e => e.HelperId).HasColumnName("helperId");
            entity.Property(e => e.MomId).HasColumnName("momId");
            entity.Property(e => e.Subject)
                .HasMaxLength(200)
                .HasColumnName("subject");
            entity.Property(e => e.TicketId).HasColumnName("ticketId");

            entity.HasOne(d => d.Child).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("childLink");

            entity.HasOne(d => d.Helper).WithMany(p => p.Reports)
                .HasForeignKey(d => d.HelperId)
                .HasConstraintName("helperLink1");

            entity.HasOne(d => d.Mom).WithMany(p => p.Reports)
                .HasForeignKey(d => d.MomId)
                .HasConstraintName("momLink1");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Reports)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("ticketLink2");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reviews");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.HelperId).HasColumnName("helperId");
            entity.Property(e => e.MomId).HasColumnName("momId");
            entity.Property(e => e.Stars).HasColumnName("stars");
            entity.Property(e => e.Text)
                .HasMaxLength(2000)
                .HasColumnName("text");

            entity.HasOne(d => d.Helper).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HelperId)
                .HasConstraintName("helperLink");

            entity.HasOne(d => d.Mom).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MomId)
                .HasConstraintName("momLink");
        });

        modelBuilder.Entity<Spec>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChildId).HasColumnName("childId");
            entity.Property(e => e.Description)
                .HasMaxLength(2000)
                .HasColumnName("description");
            entity.Property(e => e.HelperId).HasColumnName("helperId");
            entity.Property(e => e.LogForm)
                .HasMaxLength(4000)
                .HasColumnName("logForm");
            entity.Property(e => e.MomId).HasColumnName("momId");
            entity.Property(e => e.ReviewId).HasColumnName("reviewID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Child).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("children2");

            entity.HasOne(d => d.Helper).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.HelperId)
                .HasConstraintName("Helpers1");

            entity.HasOne(d => d.Mom).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.MomId)
                .HasConstraintName("moms1");

            entity.HasOne(d => d.Review).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ReviewId)
                .HasConstraintName("reviewLink");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
