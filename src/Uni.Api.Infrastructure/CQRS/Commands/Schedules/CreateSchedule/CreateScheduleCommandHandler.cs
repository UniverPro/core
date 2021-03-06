﻿using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Uni.Api.DataAccess.Contexts;
using Uni.Api.DataAccess.Models;
using Uni.Api.Infrastructure.Interfaces.CQRS.Commands;

namespace Uni.Api.Infrastructure.CQRS.Commands.Schedules.CreateSchedule
{
    [UsedImplicitly]
    public class CreateScheduleCommandHandler : ICommandHandler<CreateScheduleCommand, int>
    {
        private readonly UniDbContext _dbContext;

        public CreateScheduleCommandHandler([NotNull] UniDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> Handle(
            CreateScheduleCommand command,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    // TODO: check SubjectId & TeacherId exists
                    var schedule = new Schedule
                    {
                        SubjectId = command.SubjectId,
                        TeacherId = command.TeacherId,
                        StartTime = command.StartTime,
                        Duration = command.Duration,
                        LessonType = command.LessonType,
                        AudienceNumber = command.AudienceNumber
                    };

                    _dbContext.Schedules.Add(schedule);

                    await _dbContext.SaveChangesAsync(cancellationToken);

                    transaction.Commit();

                    return schedule.Id;
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
