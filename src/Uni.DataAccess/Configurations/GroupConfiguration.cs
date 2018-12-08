﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uni.Core;
using Uni.DataAccess.Models;

namespace Uni.DataAccess.Configurations
{
    [UsedImplicitly]
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable(nameof(Group));

            builder.HasIndex(e => e.Name);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(Consts.MaxNameLength);

            builder.HasOne(d => d.Faculty)
                .WithMany(p => p.Groups)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Faculty");
        }
    }
}
