namespace CleanArch.Domain.Filters;

public sealed record SampleFilter(bool? IsActive = null, int Page = 1, int PageSize = 10);
