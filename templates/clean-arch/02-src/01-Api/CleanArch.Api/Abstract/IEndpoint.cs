namespace CleanArch.Api.Abstract;

internal interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
