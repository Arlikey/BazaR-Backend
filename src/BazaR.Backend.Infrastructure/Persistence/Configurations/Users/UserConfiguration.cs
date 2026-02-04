using System;
using System.Linq;
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

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new UserId(value))
            .ValueGeneratedNever();

        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();

            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

       
        builder.Property<HashSet<UserRole>>("_roles")
            .HasColumnName("roles")
            .HasColumnType("text")
            .HasConversion(
                v => string.Join(',', v.Select(r => ((int)r).ToString())),
                v => string.IsNullOrWhiteSpace(v)
                    ? new HashSet<UserRole>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => (UserRole)int.Parse(s))
                        .ToHashSet()
            )
            .Metadata.SetValueComparer(new ValueComparer<HashSet<UserRole>>(
                (a, b) => a.SetEquals(b),
                v => v.Aggregate(0, (h, r) => HashCode.Combine(h, (int)r)),
                v => v.ToHashSet()
            ));
    }
}
