using MediatR;

namespace BugTracker.Main.Features.Backlog.Endpoints;

internal static class ImportProjectEndpoint
{
    public static TRouteBuilder MapImportProjectEndpoints<TRouteBuilder>(this TRouteBuilder app)
        where TRouteBuilder : IEndpointRouteBuilder
    {
        app.MapPost(
            "/api/v1/_/{OwnerKey}/import",
            async (string ownerKey,
            IFormFile file,
            IMediator mediator,
            CancellationToken ct) =>
            {
                var inputStream = file.OpenReadStream();
                await using var disposable = inputStream.ConfigureAwait(false);
                Mediator.Project.Import.Request request = new(ownerKey, inputStream);
                var response = await mediator.Send(request, ct);
                return response.Match<IResult>(
                    _ => TypedResults.Ok(),
                    err => throw err.ToException());
            });

        return app;
    }
}
