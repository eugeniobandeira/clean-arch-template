namespace CleanArch.Application.Common.Commands;

public sealed record UpdateCommand<T>(Guid Id, T Dto);
