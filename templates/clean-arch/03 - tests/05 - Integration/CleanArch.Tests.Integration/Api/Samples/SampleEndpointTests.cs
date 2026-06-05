using CleanArch.Application.Common.Response;
using CleanArch.Application.Features.Samples;
using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CleanArch.Tests.Integration.Api.Samples;

public sealed class SampleEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GET_samples_ShouldReturn200()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/samples");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        ApiListResponse<SampleResponse>? body = await response.Content.ReadFromJsonAsync<ApiListResponse<SampleResponse>>();
        body.Should().NotBeNull();
        body!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task POST_samples_ShouldReturn201_WhenValid()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest();

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/samples", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        SampleResponse? created = await response.Content.ReadFromJsonAsync<SampleResponse>();
        created.Should().NotBeNull();
        created!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task POST_samples_ShouldReturn422_WhenInvalid()
    {
        CreateSampleRequest request = SampleBuilder.BuildInvalidRequest();

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/samples", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GET_samples_ById_ShouldReturn404_WhenNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"/api/v1/samples/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_samples_ById_ShouldReturn200_WhenFound()
    {
        CreateSampleRequest request = SampleBuilder.BuildRequest();
        HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/v1/samples", request);
        SampleResponse? created = await createResponse.Content.ReadFromJsonAsync<SampleResponse>();

        HttpResponseMessage response = await _client.GetAsync($"/api/v1/samples/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        SampleResponse? fetched = await response.Content.ReadFromJsonAsync<SampleResponse>();
        fetched!.Id.Should().Be(created.Id);
    }
}
