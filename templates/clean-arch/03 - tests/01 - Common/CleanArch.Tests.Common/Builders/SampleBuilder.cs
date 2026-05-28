using Bogus;
using CleanArch.Application.Features.Samples.DTOs;
using CleanArch.Domain.Entities;

namespace CleanArch.Tests.Common.Builders;

public static class SampleBuilder
{
    private static readonly Faker Faker = new("pt_BR");

    public static Sample Build() =>
        Sample.Create(
            Faker.Commerce.ProductName(),
            Faker.Commerce.ProductDescription());

    public static List<Sample> BuildList(int count = 3) =>
        Enumerable.Range(0, count).Select(_ => Build()).ToList();

    public static CreateSampleRequest BuildRequest() =>
        new(
            Faker.Commerce.ProductName(),
            Faker.Commerce.ProductDescription());

    public static CreateSampleRequest BuildInvalidRequest() =>
        new(string.Empty, string.Empty);
}
