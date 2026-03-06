using BazaR.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> b)
    {
        b.ToTable("auth_users");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        b.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        b.Property(x => x.IsBlocked)
            .HasColumnName("is_blocked")
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        
        b.OwnsOne(x => x.Email, e =>
        {
            e.WithOwner();

            e.Property(p => p.Value)  
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();

            // Уникальный индекс на email
            e.HasIndex(p => p.Value)
                .IsUnique()
                .HasDatabaseName("ux_auth_users_email");
        });

        // Refresh Tokens
        b.OwnsMany(x => x.RefreshTokens, rt =>
        {
            rt.ToTable("auth_refresh_tokens");
            rt.WithOwner().HasForeignKey("auth_user_id");

            rt.Property<Guid>("auth_user_id")
              .HasColumnName("auth_user_id")
              .IsRequired();

            rt.HasKey(x => x.Id);
            rt.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            rt.Property(x => x.TokenHash)
                .HasColumnName("token_hash")
                .IsRequired();

            rt.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            rt.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();

            rt.Property(x => x.RevokedAt)
                .HasColumnName("revoked_at");

            rt.HasIndex(x => x.TokenHash)
              .IsUnique()
              .HasDatabaseName("ux_auth_refresh_tokens_hash");
        });

     
        var nav = b.Metadata.FindNavigation(nameof(AuthUser.RefreshTokens))!;
        nav.SetPropertyAccessMode(PropertyAccessMode.Field);
        nav.SetField("_refreshTokens");

        
        b.Ignore("DomainEvents");
    }
}