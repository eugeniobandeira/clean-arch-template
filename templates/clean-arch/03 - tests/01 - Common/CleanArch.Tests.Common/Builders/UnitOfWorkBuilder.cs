using CleanArch.Domain.Interfaces.UnitOfWork;
using Moq;

namespace CleanArch.Tests.Common.Builders;

public static class UnitOfWorkBuilder
{
    public static Mock<IUnitOfWork> BuildMock()
    {
        Mock<IUnitOfWork> mock = new();
        mock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mock;
    }

    public static IUnitOfWork Build() => BuildMock().Object;
}
