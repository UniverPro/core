﻿using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Uni.Api.DataAccess.Contexts;
using Uni.Api.DataAccess.Models;
using Uni.Api.Infrastructure.Interfaces.CQRS.Commands;

namespace Uni.Api.Infrastructure.CQRS.Commands.Faculties.CreateFaculty
{
    [UsedImplicitly]
    public class CreateFacultyCommandHandler : ICommandHandler<CreateFacultyCommand, int>
    {
        private readonly UniDbContext _dbContext;

        public CreateFacultyCommandHandler([NotNull] UniDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> Handle(
            CreateFacultyCommand command,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // TODO: check UniversityId exists
                    var faculty = new Faculty
                    {
                        UniversityId = command.UniversityId,
                        Name = command.Name,
                        ShortName = command.ShortName,
                        Description = command.Description
                    };

                    _dbContext.Faculties.Add(faculty);

                    await _dbContext.SaveChangesAsync(cancellationToken);

                    transaction.Commit();

                    return faculty.Id;
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
