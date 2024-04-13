﻿using Cramming.UseCases.Topics;
using Cramming.UseCases.Topics.CreateTag;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cramming.API.Topics
{
    public class CreateTag : EndpointBase
    {
        public const string Route = "/Topics/{TopicId}/Tags";

        public static string BuildRoute(Guid topicId) => Route.Replace("{TopicId}", topicId.ToString());

        public override void Configure(WebApplication app)
        {
            app.MapPost(Route, HandleAsync)
                .WithOpenApi()
                .WithName(nameof(CreateTag))
                .WithTags("Topics")
                .WithSummary("Create a new tag");
        }

        private async Task<Results<Created<TagDto>, NotFound>> HandleAsync(
            [FromBody] CreateTagRequest request,
            Guid topicId,
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = new CreateTagCommand(topicId, request.Name, request.Colour);

            var result = await mediator.Send(command, cancellationToken);

            if (result.Status == HttpStatusCode.NotFound)
                return TypedResults.NotFound();

            return TypedResults.Created(string.Empty, result.Value);
        }
    }
}
