﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Uni.Api.Core.Exceptions;
using Uni.Api.Infrastructure.CQRS.Commands.Groups.CreateGroup;
using Uni.Api.Infrastructure.CQRS.Commands.Groups.RemoveGroup;
using Uni.Api.Infrastructure.CQRS.Commands.Groups.UpdateGroup;
using Uni.Api.Infrastructure.CQRS.Queries.Groups.FindGroupById;
using Uni.Api.Infrastructure.CQRS.Queries.Groups.FindGroups;
using Uni.Api.Shared.Requests;
using Uni.Api.Shared.Requests.Filters;
using Uni.Api.Shared.Responses;

namespace Uni.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GroupsController(
            [NotNull] IMapper mapper,
            [NotNull] IMediator mediator
            )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        ///     Get all groups
        /// </summary>
        /// <param name="model">Query specific filters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of group objects.</returns>
        [HttpGet]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<GroupResponseModel>))]
        [SwaggerResponse(404, "Not Found", typeof(ErrorResponseModel))]
        public async Task<IEnumerable<GroupResponseModel>> GetGroups(
            [FromQuery] ListGroupsRequestModel model,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = _mapper.Map<FindGroupsQuery>(model);

            var groups = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<IEnumerable<GroupResponseModel>>(groups);

            return response;
        }

        /// <summary>
        ///     Searches the group by id
        /// </summary>
        /// <param name="groupId">Group unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Group object</returns>
        [HttpGet("{groupId:int:min(1)}")]
        [SwaggerResponse(200, "Success", typeof(GroupResponseModel))]
        [SwaggerResponse(404, "Not Found", typeof(ErrorResponseModel))]
        public async Task<GroupResponseModel> GetGroup(
            int groupId,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = new FindGroupByIdQuery(groupId);

            var group = await _mediator.Send(query, cancellationToken);

            if (group == null)
            {
                throw new NotFoundException(nameof(group), groupId);
            }

            var response = _mapper.Map<GroupResponseModel>(group);

            return response;
        }

        /// <summary>
        ///     Creates a new group
        /// </summary>
        /// <param name="model">Group object containing the data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created group object</returns>
        [HttpPost]
        [SwaggerResponse(200, "Success", typeof(GroupResponseModel))]
        [SwaggerResponse(404, "Not Found", typeof(ErrorResponseModel))]
        public async Task<GroupResponseModel> PostGroup(
            [FromForm] GroupRequestModel model,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CreateGroupCommand(
                model.Name,
                model.FacultyId,
                model.CourseNumber
            );

            var groupId = await _mediator.Send(command, cancellationToken);

            var query = new FindGroupByIdQuery(groupId);

            var group = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<GroupResponseModel>(group);

            return response;
        }

        /// <summary>
        ///     Updates the group by id
        /// </summary>
        /// <param name="groupId">Group unique identifier</param>
        /// <param name="model">Group object containing the new data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated group object</returns>
        [HttpPut("{groupId:int:min(1)}")]
        [SwaggerResponse(200, "Success", typeof(GroupResponseModel))]
        [SwaggerResponse(404, "Not Found", typeof(ErrorResponseModel))]
        public async Task<GroupResponseModel> PutGroup(
            int groupId,
            [FromForm] GroupRequestModel model,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new UpdateGroupCommand(
                groupId,
                model.Name,
                model.FacultyId,
                model.CourseNumber
            );

            await _mediator.Send(command, cancellationToken);

            var query = new FindGroupByIdQuery(groupId);

            var group = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<GroupResponseModel>(group);

            return response;
        }

        /// <summary>
        ///     Deletes the group by id
        /// </summary>
        /// <param name="groupId">Group unique identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpDelete("{groupId:int:min(1)}")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(404, "Not Found", typeof(ErrorResponseModel))]
        public async Task DeleteGroup(
            int groupId,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new RemoveGroupCommand(groupId);

            await _mediator.Send(command, cancellationToken);
        }
    }
}
