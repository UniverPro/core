﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uni.Core;
using Uni.DataAccess.Models;

namespace Uni.DataAccess.Configurations
{
    [UsedImplicitly]
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.ToTable(nameof(University));

            builder.HasIndex(e => e.Name)
                .IsUnique();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(Consts.MaxNameLength);

            builder.Property(e => e.ShortName)
                .HasMaxLength(Consts.MaxShortNameLength);
        }
    }
}
