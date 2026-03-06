using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        // === Id ===
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();

        // === Email (VO) ===
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(Email.MaxLength)
                .IsRequired()
                .HasComment("Normalized email (lowercase, trimmed)");

            //  индекс на owned property
            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("ix_users_email");

            email.WithOwner();
        });

        // === FullName (VO) ===
        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(FullName.MaxPartLength)
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(FullName.MaxPartLength)
                .IsRequired();

            // индексы на owned свойства
            name.HasIndex(n => new { n.FirstName, n.LastName })
                .HasDatabaseName("ix_users_full_name");

            name.HasIndex(n => n.LastName)
                .HasDatabaseName("ix_users_last_name");

            name.WithOwner();
        });

        // === PhoneNumber (nullable VO) ===
        builder.OwnsOne(x => x.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone")
                .HasMaxLength(PhoneNumber.MaxLength);

            //  уникальный индекс с фильтром
            phone.HasIndex(p => p.Value)
                .IsUnique()
                .HasDatabaseName("ix_users_phone")
                .HasFilter("phone IS NOT NULL");

            phone.WithOwner();
        });

        //  важно: owned навигация nullable
        builder.Navigation(x => x.Phone).IsRequired(false);

        // === Avatar (1:1) ===
        builder.HasOne<UserAvatar>("_avatar")
            .WithOne()
            .HasForeignKey<UserAvatar>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_avatar")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // === Status ===
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        // === CreatedAt / UpdatedAt ===
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.LastLoginAt)
            .HasColumnName("last_login_at")
            .IsRequired(false);

        // === Roles (HashSet<UserRole> -> CSV) ===
        builder.Ignore(x => x.Roles);

        builder.Property<HashSet<UserRole>>("_roles")
            .HasColumnName("roles")
            .HasColumnType("varchar(255)")
            .HasConversion(
                v => string.Join(',', v.Select(r => r.ToString())),
                v => string.IsNullOrWhiteSpace(v)
                    ? new HashSet<UserRole>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => Enum.Parse<UserRole>(s))
                        .ToHashSet()
            )
            .Metadata.SetValueComparer(new ValueComparer<HashSet<UserRole>>(
                (a, b) => a!.SetEquals(b!),
                v => v.Aggregate(0, (hash, r) => hash ^ r.GetHashCode()),
                v => new HashSet<UserRole>(v)
            ));

        // === индексы по простым полям ===
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_users_status");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_users_created_at");

        builder.HasIndex(x => x.LastLoginAt)
            .HasDatabaseName("ix_users_last_login_at");

        // === Ignore domain events ===
        builder.Ignore(x => x.DomainEvents);
    }
}