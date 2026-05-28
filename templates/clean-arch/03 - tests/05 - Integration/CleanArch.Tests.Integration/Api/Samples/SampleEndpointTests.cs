using System.Net;
using System.Net.Http.Json;
using CleanArch.Application.Features.Samples.DTOs;
using CleanArch.Tests.Common.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CleanArch.Tests.Integration.Api.Samples;

public sealed class SampleEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GET_samples_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/samples");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task POST_samples_ShouldReturn201_WhenValid()
    {
        var request = SampleBuilder.BuildRequest();

        var response = await _client.PostAsJsonAsync("/api/samples", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<SampleResponse>();
        created.Should().NotBeNull();
        created!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task POST_samples_ShouldReturn400_WhenInvalid()
    {
        var request = SampleBuilder.BuildInvalidRequest();

        var response = await _client.PostAsJsonAsync("/api/samples", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_samples_ById_ShouldReturn404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/samples/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
