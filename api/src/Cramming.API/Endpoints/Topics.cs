﻿using Cramming.API.Endpoints.Extensions;
using Cramming.Application.Common.Interfaces;
using Cramming.Application.Topics.Commands;
using Cramming.Application.Topics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cramming.API.Endpoints
{
    public class Topics : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            var group = app.MapGroup(this);
            
            group.MapGet(GetTopics)
                .Produces<IPaginatedList<TopicBriefDto>>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The page number of the result set.";
                    operation.Parameters[1].Description = "The size of each page in the result set.";
                    return new(operation)
                    {
                        Summary = "Retrieve paginated list of topics",
                        Description = "Endpoint to retrieve a paginated list of topics in the application."
                    };
                });

            group.MapGet(GetTopicById, "{topicId}")
                .Produces<TopicDetailDto>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The ID of the topic to retrieve.";
                    return new(operation)
                    {
                        Summary = "Retrieve topic details by ID",
                        Description = "Endpoint to retrieve the details of a specific topic in the application by its ID."
                    };
                });

            group.MapPost(CreateTopic)
                .Produces<CreateTopicResultDto>(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Create a new topic",
                    Description = "Endpoint to create a new topic in the application."
                });

            group.MapPost(AssociateTag, "{topicId}/tags")
                .Produces<AssociateTagResultDto>(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The ID of the topic to associate the tag with.";
                    return new(operation)
                    {
                        Summary = "Associate tag with topic",
                        Description = "Endpoint to associate a tag with a specific topic in the application."
                    };
                });

            group.MapDelete(DisassociateTag, "{topicId}/tags/{tagId}")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The ID of the topic from which the tag is to be disassociated.";
                    operation.Parameters[1].Description = "The ID of the tag to be disassociated from the topic.";
                    return new(operation)
                    {
                        Summary = "Disassociate tag from topic",
                        Description = "Endpoint to disassociate a tag from a specific topic in the application."
                    }; 
                });

            group.MapPost(OverrideQuestions, "{topicId}/questions:override")
                .Produces(StatusCodes.Status201Created)
                .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
                .WithOpenApi(operation =>
                {
                    operation.Parameters[0].Description = "The ID of the topic for which the questions are to be overridden.";
                    return new(operation)
                    {
                        Summary = "Override questions for a topic",
                        Description = "Endpoint to override the questions associated with a specific topic in the application."
                    };
                });
        }

        public async Task<IPaginatedList<TopicBriefDto>> GetTopics(ISender sender, [AsParameters] GetTopicsQuery query)
        {
            return await sender.Send(query);
        }

        public async Task<TopicDetailDto?> GetTopicById(ISender sender, Guid topicId)
        {
            return await sender.Send(new GetTopicByIdQuery(topicId));
        }

        public async Task<IResult> CreateTopic(ISender sender, CreateTopicCommand command)
        {
            var created = await sender.Send(command);
            return Results.CreatedAtRoute(nameof(GetTopicById), new { topicId = created.Id }, created);
        }

        public async Task<IResult> AssociateTag(ISender sender, Guid topicId, AssociateTagCommand command)
        {
            if (topicId != command.TopicId) return Results.BadRequest();
            var created = await sender.Send(command);
            return Results.Created(string.Empty, created);
        }

        public async Task<IResult> DisassociateTag(ISender sender, Guid topicId, Guid tagId)
        {
            await sender.Send(new DisassociateTagCommand(topicId, tagId));
            return Results.NoContent();
        }

        public async Task<IResult> OverrideQuestions(ISender sender, Guid topicId, OverrideQuestionsCommand command)
        {
            if (topicId != command.TopicId) return Results.BadRequest();
            await sender.Send(command);
            return Results.Created();
        }
    }
}
