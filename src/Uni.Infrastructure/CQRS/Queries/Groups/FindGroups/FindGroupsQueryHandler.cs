﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Uni.DataAccess.Contexts;
using Uni.DataAccess.Models;
using Uni.Infrastructure.Interfaces.CQRS.Queries;

namespace Uni.Infrastructure.CQRS.Queries.Groups.FindGroups
{
    [UsedImplicitly]
    public class FindGroupsQueryHandler : IQueryHandler<FindGroupsQuery, IEnumerable<Group>>
    {
        private readonly UniDbContext _dbContext;

        public FindGroupsQueryHandler([NotNull] UniDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Group>> Handle(
            FindGroupsQuery query,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var transaction =
                await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken))
            {
                try
                {
                    var groups = await _dbContext
                        .Groups
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    return groups;
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