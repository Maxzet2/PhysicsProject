using Microsoft.EntityFrameworkCore;
using PhysicsProject.Infrastructure.Persistence.Entities;

namespace PhysicsProject.Infrastructure.Persistence;

public sealed class PhysicsProjectDbContext : DbContext
{
    public PhysicsProjectDbContext(DbContextOptions<PhysicsProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<TestSessionEntity> TestSessions => Set<TestSessionEntity>();
    public DbSet<SessionItemEntity> SessionItems => Set<SessionItemEntity>();
    public DbSet<SubmissionEntity> Submissions => Set<SubmissionEntity>();
    public DbSet<ProblemTemplateEntity> ProblemTemplates => Set<ProblemTemplateEntity>();
    public DbSet<ProblemInstanceEntity> ProblemInstances => Set<ProblemInstanceEntity>();
    public DbSet<ChapterEntity> Chapters => Set<ChapterEntity>();
    public DbSet<SectionEntity> Sections => Set<SectionEntity>();
    public DbSet<SectionProgressEntity> SectionProgress => Set<SectionProgressEntity>();
    public DbSet<SectionTestAttemptEntity> SectionTestAttempts => Set<SectionTestAttemptEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(128);
            entity.HasIndex(e => e.UserName)
                .IsUnique();
        });

        modelBuilder.Entity<ProblemTemplateEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.TemplateType)
                .IsRequired()
                .HasMaxLength(64);
        });

        modelBuilder.Entity<ProblemInstanceEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Statement)
                .IsRequired();
            entity.Property(e => e.ParametersJson)
                .IsRequired()
                .HasColumnType("jsonb");
            entity.Property(e => e.NormalizedCorrectAnswer)
                .IsRequired();
            entity.HasOne(e => e.Template)
                .WithMany(t => t.Instances)
                .HasForeignKey(e => e.TemplateId);
        });

        modelBuilder.Entity<TestSessionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Mode)
                .IsRequired()
                .HasMaxLength(32);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Section)
                .WithMany()
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Items)
                .WithOne(i => i.Session)
                .HasForeignKey(i => i.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Submissions)
                .WithOne(s => s.Session)
                .HasForeignKey(s => s.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SessionItemEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<ProblemInstanceEntity>()
                .WithMany()
                .HasForeignKey(e => e.InstanceId);
        });

        modelBuilder.Entity<SubmissionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<SessionItemEntity>()
                .WithMany()
                .HasForeignKey(e => e.SessionItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChapterEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(e => e.Description)
                .HasMaxLength(1024);
            entity.HasMany(e => e.Sections)
                .WithOne(s => s.Chapter)
                .HasForeignKey(s => s.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SectionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(e => e.Description)
                .HasMaxLength(1024);
            entity.Property(e => e.TestTimeLimitSeconds)
                .HasDefaultValue(600);
            entity.HasOne(e => e.Template)
                .WithMany()
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SectionProgressEntity>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SectionId });
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Section)
                .WithMany()
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Attempts)
                .WithOne(a => a.Progress)
                .HasForeignKey(a => new { a.UserId, a.SectionId })
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SectionTestAttemptEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SessionId)
                .IsUnique();
        });
    }
}
