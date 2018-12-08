﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LinqBuilder.Core;
using Microsoft.EntityFrameworkCore;
using Uni.Core.Exceptions;
using Uni.DataAccess;
using Uni.DataAccess.Contexts;
using Uni.DataAccess.Models;
using Uni.Infrastructure.Interfaces.CQRS.Queries;

namespace Uni.Infrastructure.CQRS.Queries.Teachers.FindTeachers
{
    [UsedImplicitly]
    public class FindTeachersQueryHandler : IQueryHandler<FindTeachersQuery, IEnumerable<Teacher>>
    {
        private readonly UniDbContext _dbContext;

        public FindTeachersQueryHandler([NotNull] UniDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Teacher>> Handle(
            FindTeachersQuery query,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var specification = query.ToSpecification();

            using (var transaction =
                await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken))
            {
                try
                {
                    if (query.FacultyId != null)
                    {
                        var facultyId = query.FacultyId.Value;

                        var facultyExists = await _dbContext
                            .Faculties
                            .AsNoTracking()
                            .AnyAsync(x => x.Id == facultyId, cancellationToken);

                        if (!facultyExists)
                        {
                            throw new NotFoundException("faculty", facultyId);
                        }
                    }

                    var teachers = await _dbContext
                        .Teachers
                        .IncludeDefault()
                        .AsNoTracking()
                        .ExeSpec(specification)
                        .ToListAsync(cancellationToken);

                    transaction.Commit();
                    return teachers;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
