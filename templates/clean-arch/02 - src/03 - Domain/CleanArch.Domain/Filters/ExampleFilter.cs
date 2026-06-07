namespace CleanArch.Domain.Filters;

public sealed record ExampleFilter(bool? IsActive = null, int Page = 1, int PageSize = 10);
