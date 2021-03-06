﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uni.Api.Core;
using Uni.Api.DataAccess.Models;

namespace Uni.Api.DataAccess.Configurations
{
    [UsedImplicitly]
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(nameof(Permission));

            builder.HasIndex(e => e.Name)
                .IsUnique();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(Consts.MaxPermissionNameLength);
        }
    }
}
