using MediatR;

namespace BugTracker.Main.Features.Backlog.Endpoints;

internal static class ExportProjectEndpoint
{
    public static TRouteBuilder MapExportProjectEndpoints<TRouteBuilder>(this TRouteBuilder app)
        where TRouteBuilder : IEndpointRouteBuilder
    {
        app.MapGet(
            "/api/v1/_/{OwnerKey}/{ProjectKey}/export",
            async (string ownerKey,
            string projectKey,
            IMediator mediator,
            CancellationToken ct) =>
            {
                Mediator.Project.ExportProject.Request req = new(ownerKey, projectKey);
                var response = await mediator.Send(req, ct);
                return response.Match<IResult>(
                    path => TypedResults.PhysicalFile(path, "application/json", projectKey + ".json", enableRangeProcessing: true),
                    err => TypedResults.InternalServerError());
            });

        return app;
    }
}
