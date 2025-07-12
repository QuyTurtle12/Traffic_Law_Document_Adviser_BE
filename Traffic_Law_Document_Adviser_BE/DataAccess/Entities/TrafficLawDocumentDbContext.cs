using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities
{
    public class TrafficLawDocumentDbContext : DbContext
    {
        public TrafficLawDocumentDbContext(DbContextOptions<TrafficLawDocumentDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DocumentTag> DocumentTags { get; set; }
        public DbSet<DocumentTagMap> DocumentTagMaps { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<LawDocument> LawDocuments { get; set; }
        public DbSet<ChatDocument> ChatDocuments { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(e => e.Feedbacks)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.ChatHistories)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.News)
                      .WithOne(e => e.User)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // DocumentTag Configuration
            modelBuilder.Entity<DocumentTag>(entity =>
            {
                entity.HasOne(e => e.ParentTag)
                      .WithMany(e => e.ChildTags)
                      .HasForeignKey(e => e.ParentTagId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.DocumentTagMaps)
                      .WithOne(e => e.Tag)
                      .HasForeignKey(e => e.DocumentTagId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // DocumentTagMap Configuration
            modelBuilder.Entity<DocumentTagMap>(entity =>
            {
                entity.HasOne(e => e.Document)
                      .WithMany(e => e.DocumentTagMaps)
                      .HasForeignKey(e => e.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // LawDocument Configuration
            modelBuilder.Entity<LawDocument>(entity =>
            {
                entity.HasOne(e => e.Category)
                      .WithMany(e => e.LawDocuments)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.ChatDocuments)
                      .WithOne(e => e.LawDocument)
                      .HasForeignKey(e => e.LawDocumentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Expert)
                      .WithMany(e => e.LawDocuments)
                      .HasForeignKey(e => e.VerifyBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ChatDocument Configuration
            modelBuilder.Entity<ChatDocument>(entity =>
            {
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Feedback Configuration
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasOne(e => e.ChatHistoryNavigation)
                      .WithMany(e => e.Feedbacks)
                      .HasForeignKey(e => e.ChatHistory)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Base Entity Configuration for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime?>("CreatedTime")
                        .HasDefaultValueSql("GETDATE()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime?>("LastUpdatedTime")
                        .HasDefaultValueSql("GETDATE()");
                }
            }
        }
    }
}
