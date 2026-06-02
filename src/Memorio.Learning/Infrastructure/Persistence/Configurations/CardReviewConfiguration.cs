using Memorio.Learning.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Memorio.Learning.Infrastructure.Persistence.Configurations;

public sealed class CardReviewConfiguration : IEntityTypeConfiguration<CardReview>
{
    public void Configure(EntityTypeBuilder<CardReview> builder)
    {
        builder.ToTable("CardReviews");

        builder.HasKey(review => review.Id);

        builder.Property(review => review.SessionId).IsRequired();
        builder.Property(review => review.CardId).IsRequired();
        builder.Property(review => review.Rating).IsRequired().HasConversion<string>().HasMaxLength(16);
        builder.Property(review => review.ReviewedAt).IsRequired();
        builder.Property(review => review.NextReviewAt).IsRequired();

        builder.HasIndex(review => review.SessionId);
        builder.HasIndex(review => review.CardId);
        builder.HasIndex(review => review.ReviewedAt);

        builder.HasOne<StudySession>()
            .WithMany()
            .HasForeignKey(review => review.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
